using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _atomsTableName;

        public ReviewService(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
            _atomsTableName = Environment.GetEnvironmentVariable("ATOMS_TABLE_NAME") ?? "Atoms";
        }

        /// <summary>
        /// Main orchestration method that combines all business logic for getting due reviews
        /// </summary>
        public async Task<ReviewData> GetDueReviewsDataAsync(string userId, int limit, ILambdaContext context)
        {
            // Get due atoms from DynamoDB
            var dueAtoms = await GetDueAtomsAsync(userId, limit, context);

            // Sort by priority (importance * urgency)
            var sortedAtoms = dueAtoms
                .Select(atom => new
                {
                    Atom = atom,
                    Priority = CalculatePriority(atom)
                })
                .OrderByDescending(x => x.Priority)
                .Select(x => x.Atom)
                .ToList();

            // Calculate estimated review time
            var estimatedTime = CalculateEstimatedTime(sortedAtoms);

            // Get next review time
            var nextReviewTime = GetNextReviewTime(sortedAtoms);

            // Return all data in a single object
            return new ReviewData
            {
                SortedAtoms = sortedAtoms,
                EstimatedTimeMinutes = estimatedTime,
                NextReviewTime = nextReviewTime
            };
        }

        public async Task<List<ReviewAtom>> GetDueAtomsAsync(string userId, int limit, ILambdaContext context)
        {
            var currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            context.Logger.LogLine($"Getting due atoms for user {userId}, current time: {currentTime}, limit: {limit}");

            // Sử dụng UserReviewDateIndex thay vì UserIdIndex để tối ưu hóa query
            var request = new QueryRequest
            {
                TableName = _atomsTableName,
                IndexName = "UserReviewDateIndex", // Index tối ưu với sort key là NextReviewDate
                KeyConditionExpression = "UserId = :userId AND NextReviewDate <= :currentTime",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":userId"] = new AttributeValue { S = userId },
                    [":currentTime"] = new AttributeValue { S = currentTime }
                },
                Limit = Math.Max(limit * 2, 50), // Đảm bảo limit tối thiểu
                ScanIndexForward = true // Sắp xếp theo thứ tự tăng dần của NextReviewDate
            };

            var dueAtoms = new List<ReviewAtom>();
            var maxIterations = 10; // Giới hạn số lần lặp để tránh vòng lặp vô tận
            var iterationCount = 0;
            var consecutiveEmptyResults = 0;

            QueryResponse response;

            do
            {
                iterationCount++;

                // Kiểm tra giới hạn iterations
                if (iterationCount > maxIterations)
                {
                    context.Logger.LogLine($"Breaking loop after {maxIterations} iterations to prevent infinite loop");
                    break;
                }

                try
                {
                    response = await _dynamoDbClient.QueryAsync(request);

                    context.Logger.LogLine($"Iteration {iterationCount}: Query returned {response.Items.Count} items, " +
                                         $"LastEvaluatedKey: {(response.LastEvaluatedKey != null ? "present" : "null")}");

                    // Kiểm tra nếu có kết quả trống liên tiếp
                    if (response.Items.Count == 0)
                    {
                        consecutiveEmptyResults++;
                        context.Logger.LogLine($"Empty result #{consecutiveEmptyResults}");

                        // Nếu có quá nhiều kết quả trống liên tiếp, thoát vòng lặp
                        if (consecutiveEmptyResults >= 3)
                        {
                            context.Logger.LogLine("Too many consecutive empty results, breaking loop");
                            break;
                        }
                    }
                    else
                    {
                        consecutiveEmptyResults = 0; // Reset counter khi có kết quả
                    }

                    // Xử lý các items trả về
                    foreach (var item in response.Items)
                    {
                        try
                        {
                            var atom = MapDynamoDbItemToReviewAtom(item);

                            // Double-check điều kiện due date (phòng trường hợp có sai lệch thời gian)
                            if (DateTime.TryParse(atom.NextReviewDate, out var nextReviewDate))
                            {
                                if (nextReviewDate <= DateTime.UtcNow)
                                {
                                    dueAtoms.Add(atom);

                                    if (dueAtoms.Count >= limit)
                                    {
                                        context.Logger.LogLine($"Reached target limit of {limit} atoms, breaking loop");
                                        return dueAtoms.Take(limit).ToList();
                                    }
                                }
                                else
                                {
                                    context.Logger.LogLine($"Atom {atom.AtomId} not due yet: {atom.NextReviewDate}");
                                }
                            }
                            else
                            {
                                // Nếu không parse được date, vẫn add để tránh bỏ sót
                                dueAtoms.Add(atom);
                                context.Logger.LogLine($"Warning: Could not parse NextReviewDate for atom {atom.AtomId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            context.Logger.LogLine($"Error mapping item to ReviewAtom: {ex.Message}");
                            // Tiếp tục với item tiếp theo thay vì throw exception
                        }
                    }

                    // Nếu đã đủ số lượng cần thiết, thoát vòng lặp
                    if (dueAtoms.Count >= limit)
                    {
                        context.Logger.LogLine($"Collected enough atoms ({dueAtoms.Count}), breaking loop");
                        break;
                    }

                    // Kiểm tra LastEvaluatedKey để quyết định có tiếp tục không
                    if (response.LastEvaluatedKey != null)
                    {
                        // Log chi tiết LastEvaluatedKey để debug
                        var lastKeyJson = System.Text.Json.JsonSerializer.Serialize(
                            response.LastEvaluatedKey.ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value.S ?? kvp.Value.N ?? kvp.Value.B?.ToString() ?? "unknown"
                            )
                        );
                        context.Logger.LogLine($"Setting ExclusiveStartKey: {lastKeyJson}");

                        request.ExclusiveStartKey = response.LastEvaluatedKey;
                    }
                    else
                    {
                        context.Logger.LogLine("No more items to fetch (LastEvaluatedKey is null)");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error during DynamoDB query: {ex.Message}");
                    throw; // Re-throw để caller có thể xử lý
                }

            } while (response.LastEvaluatedKey != null && dueAtoms.Count < limit);

            context.Logger.LogLine($"Returning {dueAtoms.Count} due atoms after {iterationCount} iterations");

            // Trả về đúng số lượng yêu cầu
            return dueAtoms.Take(limit).ToList();
        }

        // Helper method để parse và validate date (optional)
        private bool IsAtomDueForReview(string nextReviewDateString, DateTime currentTime)
        {
            if (DateTime.TryParse(nextReviewDateString, out var nextReviewDate))
            {
                return nextReviewDate <= currentTime;
            }

            // Nếu không parse được, coi như due để tránh bỏ sót
            return true;
        }

        private ReviewAtom MapDynamoDbItemToReviewAtom(Dictionary<string, AttributeValue> item)
        {
            return new ReviewAtom
            {
                AtomId = item.TryGetValue("AtomId", out var atomId) ? atomId.S : null,
                UserId = item.TryGetValue("UserId", out var userId) ? userId.S : null,
                Content = item.TryGetValue("Content", out var content) ? content.S : null,
                CreatedAt = item.TryGetValue("CreatedAt", out var createdAt) ? createdAt.S : null,
                UpdatedAt = item.TryGetValue("UpdatedAt", out var updatedAt) ? updatedAt.S : null,
                NextReviewDate = item.TryGetValue("NextReviewDate", out var nextReviewDate) ? nextReviewDate.S : null,
                LastReviewDate = item.TryGetValue("LastReviewDate", out var lastReviewDate) ? lastReviewDate.S : null,
                ReviewCount = ParseInt(item.TryGetValue("ReviewCount", out var reviewCount) ? reviewCount : null),
                DifficultyScore = ParseDecimal(item.TryGetValue("DifficultyScore", out var difficultyScore) ? difficultyScore : null),
                ImportanceScore = ParseDecimal(item.TryGetValue("ImportanceScore", out var importanceScore) ? importanceScore : null),
                NoteId = item.TryGetValue("NoteId", out var noteId) ? noteId.S : null
            };
        }

        private bool IsAtomDueForReview(string nextReviewDate)
        {
            if (string.IsNullOrEmpty(nextReviewDate))
                return true;

            return DateTime.TryParse(nextReviewDate, out var reviewDate) && 
                   reviewDate <= DateTime.UtcNow;
        }

        public double CalculatePriority(ReviewAtom atom)
        {
            var importance = (double)(atom.ImportanceScore ?? 0.5m);
            var urgency = CalculateUrgencyScore(atom.NextReviewDate);
            return importance * urgency;
        }

        private double CalculateUrgencyScore(string nextReviewDate)
        {
            if (string.IsNullOrEmpty(nextReviewDate))
                return 1.0;

            if (!DateTime.TryParse(nextReviewDate, out var reviewDate))
                return 0.5;

            var currentTime = DateTime.UtcNow;
            
            if (reviewDate <= currentTime)
            {
                var overdueHours = (currentTime - reviewDate).TotalHours;
                return Math.Min(1.0, 0.5 + (overdueHours / 48.0));
            }
            
            return 0.1;
        }

        public int CalculateEstimatedTime(List<ReviewAtom> atoms)
        {
            var totalMinutes = 0.0;

            foreach (var atom in atoms)
            {
                var baseTime = 1.5;
                var difficultyMultiplier = 1.0 + ((double)(atom.DifficultyScore ?? 0.5m) - 0.5);
                var reviewCountAdjustment = Math.Max(0.5, 1.0 - (atom.ReviewCount * 0.1));
                
                totalMinutes += baseTime * difficultyMultiplier * reviewCountAdjustment;
            }

            return (int)Math.Ceiling(totalMinutes);
        }

        public string GetNextReviewTime(List<ReviewAtom> atoms)
        {
            if (!atoms.Any())
                return null;

            return DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        private decimal ParseDecimal(AttributeValue attributeValue)
        {
            if (attributeValue?.N == null)
                return 0.5m;

            return decimal.TryParse(attributeValue.N, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) 
                ? result 
                : 0.5m;
        }

        private int ParseInt(AttributeValue attributeValue)
        {
            if (attributeValue?.N == null)
                return 0;

            return int.TryParse(attributeValue.N, out var result) ? result : 0;
        }
    }
}
