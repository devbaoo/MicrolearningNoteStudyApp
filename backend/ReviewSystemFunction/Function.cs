using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ReviewSystemFunction.Handlers;
using ReviewSystemFunction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.Responses; // For ApiResponse

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ReviewSystemFunction
{
    public class Function
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IReviewService _reviewService;
        private readonly ISuperMemoService _superMemoService;
        private readonly IReviewSessionService _reviewSessionService;
        private readonly GetDueReviewsHandler _getDueReviewsHandler;
        private readonly CalculateIntervalHandler _calculateIntervalHandler;
        private readonly ReviewSessionHandler _reviewSessionHandler;

        /// <summary>
        /// Constructor that initializes the dependency chain:
        /// DynamoDBClient -> Services -> Handlers
        /// </summary>
        public Function()
        {
            // Initialize the DynamoDB client
            _dynamoDbClient = new AmazonDynamoDBClient();
            
            // Initialize the Services
            _reviewService = new ReviewService(_dynamoDbClient);
            _superMemoService = new SuperMemoService();
            _reviewSessionService = new ReviewSessionService(_dynamoDbClient, _superMemoService, _reviewService);
            
            // Initialize the Handlers with their dependencies
            _getDueReviewsHandler = new GetDueReviewsHandler(_reviewService);
            _calculateIntervalHandler = new CalculateIntervalHandler(_dynamoDbClient, _superMemoService);
            _reviewSessionHandler = new ReviewSessionHandler(_reviewSessionService);
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var method = request.RequestContext.Http.Method.ToUpper();
                var path = request.RequestContext.Http.Path;

                context.Logger.LogInformation($"Processing {method} {path}");

                // Route based on HTTP method and path
                return method switch
                {
                    // Individual endpoints
                    "GET" when path == "/reviews/due" => await _getDueReviewsHandler.HandleAsync(request, context),
                    "POST" when path == "/reviews/calculate_interval" => await _calculateIntervalHandler.HandleAsync(request, context),
                    
                    // Review session endpoints
                    "POST" when path == "/reviews/sessions" => await _reviewSessionHandler.HandleAsync(request, context),
                    "GET" when path.StartsWith("/reviews/sessions/") => await _reviewSessionHandler.HandleAsync(request, context),
                    "POST" when path.Contains("/reviews/sessions/") && path.EndsWith("/responses") => 
                        await _reviewSessionHandler.HandleAsync(request, context),
                    "PUT" when path.Contains("/reviews/sessions/") && path.EndsWith("/end") => 
                        await _reviewSessionHandler.HandleAsync(request, context),

                    "OPTIONS" => CreateCorsResponse(),

                    _ => new APIGatewayHttpApiV2ProxyResponse
                    {
                        StatusCode = 404,
                        Body = JsonConvert.SerializeObject(new ApiResponse<object>
                        {
                            Success = false,
                            Error = "Endpoint not found",
                            Timestamp = DateTime.UtcNow
                        }),
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Unhandled exception: {ex}");
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonConvert.SerializeObject(new { message = "Internal server error", error = ex.Message }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }

        private APIGatewayHttpApiV2ProxyResponse CreateCorsResponse()
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Access-Control-Allow-Origin"] = "*",
                    ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
                    ["Access-Control-Allow-Headers"] = "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"
                },
                Body = ""
            };
        }

        private APIGatewayHttpApiV2ProxyResponse CreateNotFoundResponse()
        {
            return CreateErrorResponse(404, "Endpoint not found");
        }

        private APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(int statusCode, string message)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*"
                },
                Body = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Error = message,
                    Timestamp = DateTime.UtcNow
                })
            };
        }
    }
}
