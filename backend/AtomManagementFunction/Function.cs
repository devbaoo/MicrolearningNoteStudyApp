using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Common.Repositories;
using NeuroBrain.AtomManagementFunction.Services;
using Amazon.DynamoDBv2;
using NeuroBrain.AtomManagementFunction.Handlers;
using Newtonsoft.Json;
using Amazon;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AtomManagementFunction
{
    public class Function
    {
        private readonly IAtomRepository _atomRepository;
        private readonly AtomService _atomService;

        public Function()
        {
            var dynamoDb = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1
            });
            _atomRepository = new AtomRepository(dynamoDb);
            _atomService = new AtomService(_atomRepository);
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var method = request.RequestContext.Http.Method.ToUpper();
                var path = request.RequestContext.Http.Path;
                var userId = GetUserIdFromClaims(request);

                return method switch
                {
                    "POST" when path.EndsWith("/atoms") => await new CreateAtomHandler(_atomService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/atoms") => await new GetAtomsHandler(_atomService).HandleAsync(request, userId),
                    "GET" when path.Contains("/atoms/") && !path.EndsWith("/search") && !path.EndsWith("/tags") => await new GetAtomByIdHandler(_atomService).HandleAsync(request, userId),
                    "DELETE" when path.Contains("/atoms/") => await new DeleteAtomHandler(_atomService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/atoms/search") => await new SearchAtomsHandler(_atomService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/atoms/tags") => await new GetAtomTagsHandler(_atomService).HandleAsync(request, userId),
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
                context.Logger.LogError($"Error: {ex.Message}");
                context.Logger.LogError($"StackTrace: {ex.StackTrace}");
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonConvert.SerializeObject(new { message = "Internal server error", error = ex.Message }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }

        private string GetUserIdFromClaims(APIGatewayHttpApiV2ProxyRequest request)
        {
            // Extract user ID from JWT claims or headers
            if (request.Headers?.ContainsKey("authorization") == true)
            {
                // In production, decode JWT token and extract userId
                // For now, return a placeholder
                return "user-123";
            }

            // Check for custom user header
            if (request.Headers?.ContainsKey("x-user-id") == true)
            {
                return request.Headers["x-user-id"];
            }

            // Default fallback
            return "user-123";
        }
    }
}
