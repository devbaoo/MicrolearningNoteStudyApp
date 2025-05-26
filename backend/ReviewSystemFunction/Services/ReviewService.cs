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
            _atomsTableName = Environment.GetEnvironmentVariable("ATOMS_TABLE_NAME") ?? "NeuroBrain-Atoms";
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
            
            var request = new QueryRequest
            {
                TableName = _atomsTableName,
                IndexName = "UserIdIndex",
                KeyConditionExpression = "user_id = :userId",
                FilterExpression = "next_review_date <= :currentTime AND attribute_exists(next_review_date)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":userId"] = new AttributeValue { S = userId },
                    [":currentTime"] = new AttributeValue { S = currentTime }
                },
                Limit = limit * 2,
                ScanIndexForward = true
            };

            var dueAtoms = new List<ReviewAtom>();
            QueryResponse response;

            do
            {
                response = await _dynamoDbClient.QueryAsync(request);
                
                foreach (var item in response.Items)
                {
                    var atom = MapDynamoDbItemToReviewAtom(item);
                    if (IsAtomDueForReview(atom.NextReviewDate))
                    {
                        dueAtoms.Add(atom);
                        if (dueAtoms.Count >= limit)
                            break;
                    }
                }

                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (response.LastEvaluatedKey != null && dueAtoms.Count < limit);

            return dueAtoms;
        }

        private ReviewAtom MapDynamoDbItemToReviewAtom(Dictionary<string, AttributeValue> item)
        {
            return new ReviewAtom
            {
                AtomId = item.TryGetValue("atom_id", out var atomId) ? atomId.S : null,
                UserId = item.TryGetValue("user_id", out var userId) ? userId.S : null,
                Content = item.TryGetValue("content", out var content) ? content.S : null,
                CreatedAt = item.TryGetValue("created_at", out var createdAt) ? createdAt.S : null,
                UpdatedAt = item.TryGetValue("updated_at", out var updatedAt) ? updatedAt.S : null,
                NextReviewDate = item.TryGetValue("next_review_date", out var nextReviewDate) ? nextReviewDate.S : null,
                LastReviewDate = item.TryGetValue("last_review_date", out var lastReviewDate) ? lastReviewDate.S : null,
                ReviewCount = ParseInt(item.TryGetValue("review_count", out var reviewCount) ? reviewCount : null),
                DifficultyScore = ParseDecimal(item.TryGetValue("difficulty_score", out var difficultyScore) ? difficultyScore : null),
                ImportanceScore = ParseDecimal(item.TryGetValue("importance_score", out var importanceScore) ? importanceScore : null),
                NoteId = item.TryGetValue("note_id", out var noteId) ? noteId.S : null
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
