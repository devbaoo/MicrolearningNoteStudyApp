using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Responses;
using Newtonsoft.Json;
using ReviewSystemFunction.Services;
using System.Text.Json;

namespace ReviewSystemFunction.Handlers
{
    public class GetDueReviewsHandler
    {
        private readonly IReviewService _reviewService;

        public GetDueReviewsHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Starting GetDueReviews handler at {DateTime.UtcNow}");

            var requestJson = System.Text.Json.JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            context.Logger.LogInformation($"Request GetDueReviews: {requestJson}");


            // Extract and validate parameters - Handler responsibility
            string? userId = null;
            if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("user_id"))
            {
                userId = request.QueryStringParameters["user_id"];
            }

            if (string.IsNullOrEmpty(userId))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    //test
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new ApiResponse<GetDueReviewsResponse>
                    {
                        Success = false,
                        Message = "Failed getting due review."
                    }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            // Default to 5 atoms per day for optimal learning pace
            string limitStr = "5";
            if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("limit"))
            {
                limitStr = request.QueryStringParameters["limit"];
            }

            if (!int.TryParse(limitStr, out var limit) || limit <= 0)
            {
                limit = 5;
            }

            // Ensure reasonable limit - default to 5 atoms per day, max 20
            limit = Math.Min(limit, 20);

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

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<GetDueReviewsResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Notes retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

/*        private APIGatewayProxyResponse CreateSuccessResponse(object data)
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
                    Message = message,
                    Timestamp = DateTime.UtcNow
                })
            };
        }*/
    }
}
