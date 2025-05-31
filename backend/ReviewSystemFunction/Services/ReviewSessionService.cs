using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Common.Models;
using Common.Requests;
using Common.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public class ReviewSessionService : IReviewSessionService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ISuperMemoService _superMemoService;
        private readonly IReviewService _reviewService;
        private readonly string _atomsTableName;
        private readonly string _reviewResponsesTableName;
        private readonly string _reviewSessionsTableName;

        public ReviewSessionService(IAmazonDynamoDB dynamoDbClient, ISuperMemoService superMemoService, IReviewService reviewService)
        {
            _dynamoDbClient = dynamoDbClient ?? throw new ArgumentNullException(nameof(dynamoDbClient));
            _superMemoService = superMemoService ?? throw new ArgumentNullException(nameof(superMemoService));
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _atomsTableName = Environment.GetEnvironmentVariable("ATOMS_TABLE_NAME") ?? "Atoms";
            _reviewResponsesTableName = Environment.GetEnvironmentVariable("REVIEW_RESPONSES_TABLE_NAME") ?? "ReviewResponses";
            _reviewSessionsTableName = Environment.GetEnvironmentVariable("REVIEW_SESSIONS_TABLE_NAME") ?? "ReviewSessions";
        }

        #region Start Review Session

        public async Task<StartReviewSessionResponse> StartSessionAsync(StartReviewSessionRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("Starting new review session");

            // Validate request
            if (string.IsNullOrEmpty(request?.UserId))
            {
                throw new ArgumentException("UserId is required");
            }

            // Get due atoms for review using the review service
            var dueReviewsData = await _reviewService.GetDueReviewsDataAsync(request.UserId, request.MaxAtoms, context);
            var dueAtoms = dueReviewsData.SortedAtoms;

            if (!dueAtoms.Any())
            {
                return new StartReviewSessionResponse
                {
                    SessionId = null,
                    Message = "No atoms due for review",
                    AtomsToReview = new List<ReviewAtomForSession>(),
                    EstimatedTimeMinutes = 0
                };
            }

            // Create session
            var sessionId = Guid.NewGuid().ToString();
            var session = await CreateSessionAsync(sessionId, request, dueAtoms, context);

            // Prepare atoms for review (clean data for frontend)
            var reviewAtoms = dueAtoms.Select(atom => new ReviewAtomForSession
            {
                AtomId = atom.AtomId,
                Content = atom.Content,
                Type = atom.Type ?? "concept",
                Tags = atom.Tags ?? new List<string>(),
                ImportanceScore = atom.ImportanceScore ?? 0.5m,
                ReviewCount = atom.ReviewCount
            }).ToList();

            var response = new StartReviewSessionResponse
            {
                SessionId = sessionId,
                AtomsToReview = reviewAtoms,
                TotalAtoms = reviewAtoms.Count,
                EstimatedTimeMinutes = CalculateEstimatedTime(reviewAtoms),
                SessionSettings = session.SessionSettings,
                Message = $"Review session started with {reviewAtoms.Count} atoms"
            };

            context.Logger.LogInformation($"Session {sessionId} started with {reviewAtoms.Count} atoms");
            return response;
        }

        private async Task<ReviewSession> CreateSessionAsync(
            string sessionId,
            StartReviewSessionRequest request,
            List<ReviewAtom> dueAtoms,
            ILambdaContext context)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var session = new ReviewSession
            {
                SessionId = sessionId,
                UserId = request.UserId,
                SessionType = request.SessionType ?? "regular",
                Status = "active",
                StartTime = timestamp,
                AtomsToReview = dueAtoms.Select(a => a.AtomId).ToList(),
                TotalAtoms = dueAtoms.Count,
                CompletedAtoms = 0,
                SessionSettings = new SessionSettings
                {
                    MaxAtoms = request.MaxAtoms,
                    TimeLimitMinutes = request.TimeLimitMinutes,
                    ShuffleOrder = request.ShuffleOrder ?? true,
                    ShowHints = request.ShowHints ?? false
                }
            };

            // Store in DynamoDB
            var item = new Dictionary<string, AttributeValue>
            {
                ["session_id"] = new AttributeValue { S = sessionId },
                ["user_id"] = new AttributeValue { S = request.UserId },
                ["session_type"] = new AttributeValue { S = session.SessionType },
                ["status"] = new AttributeValue { S = "active" },
                ["start_time"] = new AttributeValue { S = timestamp },
                ["total_atoms"] = new AttributeValue { N = session.TotalAtoms.ToString() },
                ["completed_atoms"] = new AttributeValue { N = "0" },
                ["atoms_to_review"] = new AttributeValue { SS = session.AtomsToReview },
                ["session_settings"] = new AttributeValue { S = JsonConvert.SerializeObject(session.SessionSettings) },
                ["ttl"] = new AttributeValue { N = ((DateTimeOffset)DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds().ToString() }
            };

            await _dynamoDbClient.PutItemAsync(new PutItemRequest
            {
                TableName = _reviewSessionsTableName,
                Item = item
            });

            return session;
        }

        #endregion

        #region Submit Review Response

        public async Task<SubmitReviewResponseResponse> SubmitResponseAsync(
            string sessionId, 
            SubmitReviewResponseRequest request, 
            ILambdaContext context)
        {
            context.Logger.LogInformation($"Submitting response for session: {sessionId}");

            // Validate request
            if (string.IsNullOrEmpty(request?.AtomId))
            {
                throw new ArgumentException("Atom ID is required");
            }

            if (request.ResponseData == null)
            {
                throw new ArgumentException("Response data is required");
            }

            if (request.ResponseData.SuccessRating < 0 || request.ResponseData.SuccessRating > 1)
            {
                throw new ArgumentException("Success rating must be between 0 and 1");
            }

            if (request.ResponseData.ResponseTimeMs < 0)
            {
                throw new ArgumentException("Response time cannot be negative");
            }

            // Verify session is valid and active
            var session = await GetSessionByIdAsync(sessionId, context);
            if (session == null)
            {
                throw new KeyNotFoundException("Review session not found");
            }

            if (session.Status != "active")
            {
                throw new InvalidOperationException($"Session is not active. Current status: {session.Status}");
            }

            // Get atom data for calculation
            var atomData = await GetAtomDataAsync(request.AtomId, context);
            if (atomData == null)
            {
                throw new KeyNotFoundException("Atom not found");
            }

            // Calculate next review interval
            var intervalResult = await _superMemoService.CalculateNextReviewIntervalAsync(
                atomData,
                request.ResponseData.SuccessRating,
                request.ResponseData.ResponseTimeMs,
                context
            );

            // Store the review response
            var responseId = await StoreReviewResponseAsync(sessionId, request, intervalResult, context);

            // Update atom with new scheduling
            await UpdateAtomSchedulingAsync(request.AtomId, intervalResult, context);

            // Update session progress
            var updatedSession = await UpdateSessionProgressAsync(sessionId, request.AtomId, context);

            var response = new SubmitReviewResponseResponse
            {
                ResponseId = responseId,
                AtomId = request.AtomId,
                NextReviewDate = intervalResult.NextReviewDate,
                NewIntervalDays = intervalResult.NewIntervalDays,
                PerformanceCategory = intervalResult.PerformanceCategory,
                RetentionProbability = intervalResult.RetentionProbability,
                ImprovementSuggestions = GenerateImprovementSuggestions(intervalResult),
                SessionProgress = new SessionProgress
                {
                    SessionId = sessionId,
                    TotalAtoms = updatedSession.TotalAtoms,
                    CompletedAtoms = updatedSession.CompletedAtoms,
                    RemainingAtoms = updatedSession.TotalAtoms - updatedSession.CompletedAtoms,
                    ProgressPercentage = (double)updatedSession.CompletedAtoms / updatedSession.TotalAtoms * 100,
                    IsComplete = updatedSession.CompletedAtoms >= updatedSession.TotalAtoms
                }
            };

            context.Logger.LogInformation($"Response recorded: {responseId}, Progress: {updatedSession.CompletedAtoms}/{updatedSession.TotalAtoms}");
            return response;
        }

        #endregion

        #region End Review Session

        public async Task<EndReviewSessionResponse> EndSessionAsync(string sessionId, ILambdaContext context)
        {
            context.Logger.LogInformation($"Ending session: {sessionId}");

            // Get session data
            var session = await GetSessionByIdAsync(sessionId, context);
            if (session == null)
            {
                throw new KeyNotFoundException("Review session not found");
            }

            if (session.Status == "completed")
            {
                throw new InvalidOperationException("Session is already completed");
            }

            // Get session statistics
            var sessionStats = await CalculateSessionStatisticsAsync(sessionId, context);

            // Update session status to completed
            await UpdateSessionStatusAsync(sessionId, "completed", context);

            var response = new EndReviewSessionResponse
            {
                SessionId = sessionId,
                CompletedAt = DateTime.UtcNow,
                SessionDurationMinutes = CalculateSessionDuration(session.StartTime),
                SessionStatistics = sessionStats,
                PerformanceSummary = GeneratePerformanceSummary(sessionStats),
                NextReviewSuggestion = await GenerateNextReviewSuggestionAsync(session.UserId, context)
            };

            context.Logger.LogInformation($"Session {sessionId} completed successfully");
            return response;
        }

        #endregion

        #region Get Session Status

        public async Task<GetSessionResponse> GetSessionAsync(string sessionId, ILambdaContext context)
        {
            var session = await GetSessionByIdAsync(sessionId, context);
            if (session == null)
            {
                throw new KeyNotFoundException("Review session not found");
            }

            var response = new GetSessionResponse
            {
                SessionId = session.SessionId,
                Status = session.Status,
                StartTime = DateTime.Parse(session.StartTime),
                TotalAtoms = session.TotalAtoms,
                CompletedAtoms = session.CompletedAtoms,
                RemainingAtoms = session.TotalAtoms - session.CompletedAtoms,
                ProgressPercentage = (double)session.CompletedAtoms / session.TotalAtoms * 100,
                SessionSettings = session.SessionSettings
            };

            return response;
        }

        #endregion

        #region Helper Methods

        public async Task<ReviewSession> GetSessionByIdAsync(string sessionId, ILambdaContext context)
        {
            try
            {
                var response = await _dynamoDbClient.GetItemAsync(new GetItemRequest
                {
                    TableName = _reviewSessionsTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["session_id"] = new AttributeValue { S = sessionId }
                    }
                });

                if (!response.IsItemSet)
                    return null;

                return new ReviewSession
                {
                    SessionId = response.Item.GetValueOrDefault("session_id")?.S,
                    UserId = response.Item.GetValueOrDefault("user_id")?.S,
                    SessionType = response.Item.GetValueOrDefault("session_type")?.S,
                    Status = response.Item.GetValueOrDefault("status")?.S,
                    StartTime = response.Item.GetValueOrDefault("start_time")?.S,
                    EndTime = response.Item.GetValueOrDefault("end_time")?.S,
                    TotalAtoms = ParseInt(response.Item.GetValueOrDefault("total_atoms")),
                    CompletedAtoms = ParseInt(response.Item.GetValueOrDefault("completed_atoms")),
                    AtomsToReview = response.Item.GetValueOrDefault("atoms_to_review")?.SS?.ToList() ?? new List<string>(),
                    SessionSettings = JsonConvert.DeserializeObject<SessionSettings>(
                        response.Item.GetValueOrDefault("session_settings")?.S ?? "{}")
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error getting session: {ex.Message}");
                return null;
            }
        }

        public async Task<ReviewAtom> GetAtomDataAsync(string atomId, ILambdaContext context)
        {
            try
            {
                var response = await _dynamoDbClient.GetItemAsync(new GetItemRequest
                {
                    TableName = _atomsTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["atom_id"] = new AttributeValue { S = atomId }
                    }
                });

                if (!response.IsItemSet)
                    return null;

                return new ReviewAtom
                {
                    AtomId = response.Item.GetValueOrDefault("atom_id")?.S,
                    UserId = response.Item.GetValueOrDefault("user_id")?.S,
                    Content = response.Item.GetValueOrDefault("content")?.S,
                    Type = response.Item.GetValueOrDefault("type")?.S ?? "concept",
                    ImportanceScore = ParseDecimal(response.Item.GetValueOrDefault("importance_score")),
                    DifficultyScore = ParseDecimal(response.Item.GetValueOrDefault("difficulty_score")),
                    CurrentInterval = ParseInt(response.Item.GetValueOrDefault("current_interval")),
                    EaseFactor = ParseDecimal(response.Item.GetValueOrDefault("ease_factor")) ?? 2.5m,
                    ReviewCount = ParseInt(response.Item.GetValueOrDefault("review_count")),
                    LastReviewDate = response.Item.GetValueOrDefault("last_review_date")?.S,
                    NextReviewDate = response.Item.GetValueOrDefault("next_review_date")?.S,
                    Tags = response.Item.GetValueOrDefault("tags")?.SS?.ToList() ?? new List<string>(),
                    CreatedAt = response.Item.GetValueOrDefault("created_at")?.S,
                    UpdatedAt = response.Item.GetValueOrDefault("updated_at")?.S,
                    NoteId = response.Item.GetValueOrDefault("note_id")?.S
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error getting atom data: {ex.Message}");
                throw;
            }
        }

        public async Task<string> StoreReviewResponseAsync(
            string sessionId,
            SubmitReviewResponseRequest request,
            CalculateIntervalResponse intervalResult,
            ILambdaContext context)
        {
            var responseId = Guid.NewGuid().ToString();
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var item = new Dictionary<string, AttributeValue>
            {
                ["response_id"] = new AttributeValue { S = responseId },
                ["session_id"] = new AttributeValue { S = sessionId },
                ["atom_id"] = new AttributeValue { S = request.AtomId },
                ["timestamp"] = new AttributeValue { S = timestamp },
                ["success_rating"] = new AttributeValue { N = request.ResponseData.SuccessRating.ToString(CultureInfo.InvariantCulture) },
                ["response_time_ms"] = new AttributeValue { N = request.ResponseData.ResponseTimeMs.ToString() },
                ["confidence_level"] = new AttributeValue { N = (request.ResponseData.ConfidenceLevel ?? 0.5).ToString(CultureInfo.InvariantCulture) },
                ["difficulty_perceived"] = new AttributeValue { N = (request.ResponseData.DifficultyPerceived ?? 0.5).ToString(CultureInfo.InvariantCulture) },
                ["review_method"] = new AttributeValue { S = request.ResponseData.ReviewMethod ?? "standard" },
                ["notes"] = new AttributeValue { S = request.ResponseData.Notes ?? "" },
                ["calculated_interval"] = new AttributeValue { N = intervalResult.NewIntervalDays.ToString() },
                ["calculated_ease_factor"] = new AttributeValue { N = intervalResult.EaseFactor.ToString(CultureInfo.InvariantCulture) },
                ["performance_category"] = new AttributeValue { S = intervalResult.PerformanceCategory },
                ["retention_probability"] = new AttributeValue { N = intervalResult.RetentionProbability.ToString(CultureInfo.InvariantCulture) },
                ["algorithm_version"] = new AttributeValue { S = intervalResult.AlgorithmVersion },
                ["ttl"] = new AttributeValue { N = ((DateTimeOffset)DateTime.UtcNow.AddDays(90)).ToUnixTimeSeconds().ToString() }
            };

            await _dynamoDbClient.PutItemAsync(new PutItemRequest
            {
                TableName = _reviewResponsesTableName,
                Item = item
            });

            return responseId;
        }

        public async Task UpdateAtomSchedulingAsync(string atomId, CalculateIntervalResponse intervalResult, ILambdaContext context)
        {
            var updateExpression = "SET current_interval = :interval, ease_factor = :ease, next_review_date = :next_date, " +
                                 "review_count = review_count + :inc, last_review_date = :last_date, " +
                                 "difficulty_score = :difficulty, updated_at = :updated_at";

            var expressionValues = new Dictionary<string, AttributeValue>
            {
                [":interval"] = new AttributeValue { N = intervalResult.NewIntervalDays.ToString() },
                [":ease"] = new AttributeValue { N = intervalResult.EaseFactor.ToString(CultureInfo.InvariantCulture) },
                [":next_date"] = new AttributeValue { S = intervalResult.NextReviewDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                [":inc"] = new AttributeValue { N = "1" },
                [":last_date"] = new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                [":difficulty"] = new AttributeValue { N = intervalResult.NewDifficultyScore.ToString(CultureInfo.InvariantCulture) },
                [":updated_at"] = new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
            };

            await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = _atomsTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["atom_id"] = new AttributeValue { S = atomId }
                },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionValues
            });
        }

        public async Task<ReviewSession> UpdateSessionProgressAsync(string sessionId, string atomId, ILambdaContext context)
        {
            var updateExpression = "ADD completed_atoms :inc SET updated_at = :updated_at";
            
            var expressionValues = new Dictionary<string, AttributeValue>
            {
                [":inc"] = new AttributeValue { N = "1" },
                [":updated_at"] = new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
            };

            await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = _reviewSessionsTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["session_id"] = new AttributeValue { S = sessionId }
                },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionValues
            });

            return await GetSessionByIdAsync(sessionId, context);
        }

        public async Task UpdateSessionStatusAsync(string sessionId, string status, ILambdaContext context)
        {
            var updateExpression = "SET #status = :status, end_time = :end_time, updated_at = :updated_at";
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            
            var expressionValues = new Dictionary<string, AttributeValue>
            {
                [":status"] = new AttributeValue { S = status },
                [":end_time"] = new AttributeValue { S = timestamp },
                [":updated_at"] = new AttributeValue { S = timestamp }
            };

            var expressionNames = new Dictionary<string, string>
            {
                ["#status"] = "status"
            };

            await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = _reviewSessionsTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["session_id"] = new AttributeValue { S = sessionId }
                },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionValues,
                ExpressionAttributeNames = expressionNames
            });
        }

        public async Task<SessionStatistics> CalculateSessionStatisticsAsync(string sessionId, ILambdaContext context)
        {
            try
            {
                var request = new QueryRequest
                {
                    TableName = _reviewResponsesTableName,
                    IndexName = "SessionResponsesIndex",
                    KeyConditionExpression = "session_id = :sessionId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":sessionId"] = new AttributeValue { S = sessionId }
                    }
                };

                var response = await _dynamoDbClient.QueryAsync(request);
                var responses = response.Items;

                if (!responses.Any())
                {
                    return new SessionStatistics();
                }

                var successRatings = responses.Select(r => double.Parse(r.GetValueOrDefault("success_rating")?.N ?? "0")).ToList();
                var responseTimes = responses.Select(r => int.Parse(r.GetValueOrDefault("response_time_ms")?.N ?? "0")).ToList();
                var performanceCategories = responses.Select(r => r.GetValueOrDefault("performance_category")?.S ?? "").ToList();

                return new SessionStatistics
                {
                    TotalResponses = responses.Count,
                    AverageSuccessRating = successRatings.Average(),
                    AverageResponseTimeMs = (int)responseTimes.Average(),
                    ExcellentCount = performanceCategories.Count(c => c == "Excellent"),
                    GoodCount = performanceCategories.Count(c => c == "Good"),
                    FairCount = performanceCategories.Count(c => c == "Fair"),
                    NeedsReviewCount = performanceCategories.Count(c => c == "Needs Review"),
                    FastestResponseMs = responseTimes.Any() ? responseTimes.Min() : 0,
                    SlowestResponseMs = responseTimes.Any() ? responseTimes.Max() : 0
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogWarning($"Could not calculate session statistics: {ex.Message}");
                return new SessionStatistics();
            }
        }

        public int CalculateEstimatedTime(List<ReviewAtomForSession> atoms)
        {
            return (int)Math.Ceiling(atoms.Count * 1.5); // 1.5 minutes per atom
        }

        private int CalculateSessionDuration(string startTime)
        {
            if (DateTime.TryParse(startTime, out var start))
            {
                return (int)(DateTime.UtcNow - start).TotalMinutes;
            }
            return 0;
        }

        public List<string> GenerateImprovementSuggestions(CalculateIntervalResponse intervalResult)
        {
            var suggestions = new List<string>();

            switch (intervalResult.PerformanceCategory)
            {
                case "Needs Review":
                    suggestions.Add("Consider reviewing the source material again");
                    suggestions.Add("Try breaking this concept into smaller parts");
                    break;
                case "Fair":
                    suggestions.Add("Practice active recall techniques");
                    suggestions.Add("Review related concepts to strengthen connections");
                    break;
                case "Good":
                    suggestions.Add("Try to explain this concept to someone else");
                    break;
                case "Excellent":
                    suggestions.Add("Great job! Consider teaching this to others");
                    break;
            }

            return suggestions;
        }

        public PerformanceSummary GeneratePerformanceSummary(SessionStatistics stats)
        {
            var overallGrade = stats.AverageSuccessRating switch
            {
                >= 0.9 => "A",
                >= 0.8 => "B",
                >= 0.7 => "C",
                >= 0.6 => "D",
                _ => "F"
            };

            return new PerformanceSummary
            {
                OverallGrade = overallGrade,
                StrengthAreas = GenerateStrengthAreas(stats),
                ImprovementAreas = GenerateImprovementAreas(stats),
                StudyRecommendations = GenerateStudyRecommendations(stats)
            };
        }

        private List<string> GenerateStrengthAreas(SessionStatistics stats)
        {
            var strengths = new List<string>();
            
            if (stats.ExcellentCount > stats.TotalResponses * 0.5)
            {
                strengths.Add("Excellent knowledge retention");
            }
            
            if (stats.AverageResponseTimeMs < 3000)
            {
                strengths.Add("Quick recall speed");
            }
            
            if (stats.AverageSuccessRating >= 0.8)
            {
                strengths.Add("Strong understanding of concepts");
            }

            return strengths.Any() ? strengths : new List<string> { "Keep practicing to build strengths" };
        }

        private List<string> GenerateImprovementAreas(SessionStatistics stats)
        {
            var improvements = new List<string>();
            
            if (stats.NeedsReviewCount > stats.TotalResponses * 0.3)
            {
                improvements.Add("Focus on concepts that need more review");
            }
            
            if (stats.AverageResponseTimeMs > 8000)
            {
                improvements.Add("Work on faster recall");
            }
            
            if (stats.AverageSuccessRating < 0.6)
            {
                improvements.Add("Review fundamental concepts");
            }

            return improvements;
        }

        private List<string> GenerateStudyRecommendations(SessionStatistics stats)
        {
            var recommendations = new List<string>();
            
            if (stats.FairCount + stats.NeedsReviewCount > stats.ExcellentCount + stats.GoodCount)
            {
                recommendations.Add("Schedule additional review sessions");
                recommendations.Add("Break difficult concepts into smaller parts");
            }
            else
            {
                recommendations.Add("Continue with current study pace");
                recommendations.Add("Consider exploring advanced topics");
            }

            return recommendations;
        }

        public async Task<string> GenerateNextReviewSuggestionAsync(string userId, ILambdaContext context)
        {
            try
            {
                // Get user's next review items
                var dueReviewsData = await _reviewService.GetDueReviewsDataAsync(userId, 1, context);
                
                if (dueReviewsData.SortedAtoms.Any())
                {
                    return "You have more atoms ready for review now!";
                }

                return "Great job! Take a break and come back later for your next review session.";
            }
            catch (Exception ex)
            {
                context.Logger.LogWarning($"Could not generate next review suggestion: {ex.Message}");
                return "Check back later for your next review session.";
            }
        }

        // Utility Methods
        private decimal? ParseDecimal(AttributeValue attributeValue)
        {
            if (attributeValue?.N == null) return null;
            return decimal.TryParse(attributeValue.N, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;
        }

        private int ParseInt(AttributeValue attributeValue)
        {
            if (attributeValue?.N == null) return 0;
            return int.TryParse(attributeValue.N, out var result) ? result : 0;
        }

        #endregion
    }
}
