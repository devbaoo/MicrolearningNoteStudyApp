using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Models;
using Common.Responses;
using Newtonsoft.Json;
using ReviewSystemFunction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Handlers
{
    public class GetDueReviewsHandler
    {
        private readonly IReviewService _reviewService;

        public GetDueReviewsHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation($"Starting GetDueReviews handler at {DateTime.UtcNow}");

                // Extract and validate parameters - Handler responsibility
                string userId = null;
                if (request.QueryStringParameters != null && request.QueryStringParameters.TryGetValue("user_id", out var userIdValue))
                {
                    userId = userIdValue;
                }
                
                if (string.IsNullOrEmpty(userId))
                {
                    return CreateErrorResponse(400, "user_id is required");
                }

                string limitStr = "50";
                if (request.QueryStringParameters != null && request.QueryStringParameters.TryGetValue("limit", out var limitValue))
                {
                    limitStr = limitValue;
                }
                
                if (!int.TryParse(limitStr, out var limit) || limit <= 0)
                {
                    limit = 50;
                }

                // Ensure reasonable limit
                limit = Math.Min(limit, 100);

                context.Logger.LogInformation($"Getting due atoms for user: {userId}, limit: {limit}");

                // Delegate to service layer for business logic and data operations
                var reviewData = await _reviewService.GetDueReviewsDataAsync(userId, limit, context);

                // Prepare response - Handler responsibility
                var response = new GetDueReviewsResponse
                {
                    DueAtoms = reviewData.SortedAtoms,
                    TotalCount = reviewData.SortedAtoms.Count,
                    ReviewLimitReached = reviewData.SortedAtoms.Count >= limit,
                    EstimatedReviewTimeMinutes = reviewData.EstimatedTimeMinutes,
                    NextReviewTime = reviewData.NextReviewTime
                };

                context.Logger.LogInformation($"Found {reviewData.SortedAtoms.Count} due atoms");

                return CreateSuccessResponse(response);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error in GetDueReviews: {ex.Message}");
                return CreateErrorResponse(500, "Failed to fetch due atoms");
            }
        }

        private APIGatewayProxyResponse CreateSuccessResponse(object data)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*",
                    ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
                    ["Access-Control-Allow-Headers"] = "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"
                },
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = data,
                    Message = "Due atoms retrieved successfully"
                })
            };
        }

        private APIGatewayProxyResponse CreateErrorResponse(int statusCode, string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*"
                },
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = false,
                    Error = message,
                    Timestamp = DateTime.UtcNow
                })
            };
        }
    }
}
