using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Common.Repositories;
using Common.Models;
using System.Net;
using UserProfileFunction.Services;
using UserProfileFunction.Handlers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserProfileFunction;
//test
//test
public class Function
{
    private readonly IHandler _getUserByIdHandler;
    private readonly IHandler _getAllUsersHandler;
    private readonly IHandler _createUserHandler;
    private readonly IHandler _updateUserHandler;
    private readonly IHandler _deleteUserHandler;
    private static IAmazonDynamoDB _dynamoDbClient;
    
    public Function()
    {
        // Initialize DynamoDB client only when actually needed (not during deployment)
        // Use lazy initialization pattern to avoid connections during class loading
        _dynamoDbClient ??= new AmazonDynamoDBClient();
        var userRepository = new UserRepository(_dynamoDbClient);
        var userService = new UserService(userRepository);
        
        // Initialize handlers
        _getUserByIdHandler = new GetUserByIdHandler(userService);
        _getAllUsersHandler = new GetAllUsersHandler(userService);
        _createUserHandler = new CreateUserHandler(userService);
        _updateUserHandler = new UpdateUserHandler(userService);
        _deleteUserHandler = new DeleteUserHandler(userService);
    }
    
    // Constructor for testing with dependency injection
    public Function(IUserRepository userRepository)
    {
        var userService = new UserService(userRepository);
        
        // Initialize handlers
        _getUserByIdHandler = new GetUserByIdHandler(userService);
        _getAllUsersHandler = new GetAllUsersHandler(userService);
        _createUserHandler = new CreateUserHandler(userService);
        _updateUserHandler = new UpdateUserHandler(userService);
        _deleteUserHandler = new DeleteUserHandler(userService);
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request in UserProfileFunction");
        
        try
        {
            //test
            // Extract the path and method for easier routing
            var path = request.RawPath.ToLower();
            var method = request.RequestContext.Http.Method.ToUpper();

            // Route the request based on HTTP method and path
            return method switch
            {
                "GET" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await _getUserByIdHandler.HandleAsync(request),
                "GET" => await _getAllUsersHandler.HandleAsync(request),
                "POST" => await _createUserHandler.HandleAsync(request),
                "PUT" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await _updateUserHandler.HandleAsync(request),
                "DELETE" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await _deleteUserHandler.HandleAsync(request),
                _ => new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Body = JsonSerializer.Serialize(new { Message = "Route not found" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                }
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error processing request: {ex.Message}");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = JsonSerializer.Serialize(new { Message = "An error occurred processing your request" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}

// Custom JSON converter for DateOnly type
public class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();
            if (DateOnly.TryParse(dateString, out var date))
            {
                return date;
            }
        }
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}
