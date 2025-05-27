using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ReviewSystemFunction.Handlers;
using ReviewSystemFunction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ReviewSystemFunction
{
    public class Function
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IReviewService _reviewService;
        private readonly GetDueReviewsHandler _getDueReviewsHandler;

        /// <summary>
        /// Constructor that initializes the dependency chain:
        /// DynamoDBClient -> ReviewService -> GetDueReviewsHandler
        /// </summary>
        public Function()
        {
            // Initialize the DynamoDB client
            _dynamoDbClient = new AmazonDynamoDBClient();
            
            // Initialize the ReviewService with the DynamoDB client
            _reviewService = new ReviewService(_dynamoDbClient);
            
            // Initialize the GetDueReviewsHandler with the ReviewService
            _getDueReviewsHandler = new GetDueReviewsHandler(_reviewService);
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
                    /* ("GET", "/reviews/due") => await _getDueReviewsHandler.HandleAsync(request, context),

                    ("OPTIONS", _) => CreateCorsResponse(),*/

                    _ => new APIGatewayHttpApiV2ProxyResponse
                    {
                        StatusCode = 404,
                        Body = JsonConvert.SerializeObject(new { message = "Endpoint not found" }),
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

        private APIGatewayProxyResponse CreateCorsResponse()
        {
            return new APIGatewayProxyResponse
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

        private APIGatewayProxyResponse CreateNotFoundResponse()
        {
            return CreateErrorResponse(404, "Endpoint not found");
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
