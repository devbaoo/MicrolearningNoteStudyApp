using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Common.Repositories;
using Common.Models;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserManagementFunction;
//test

public class Function
{
    private readonly IUserRepository _userRepository;
    
    public Function()
    {
        var dynamoDbClient = new AmazonDynamoDBClient();
        _userRepository = new UserRepository(dynamoDbClient);
    }
    
    // Constructor for testing with dependency injection
    public Function(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request in UserManagementFunction");
        
        try
        {
            //test
            // Extract the path and method for easier routing
            var path = request.RawPath.ToLower();
            var method = request.RequestContext.Http.Method.ToUpper();

            // Route the request based on HTTP method and path
            return request.RequestContext.Http.Method.ToUpper() switch
            {
                "GET" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await GetUserById(request.PathParameters["userId"]),
                "GET" => await GetAllUsers(),
                "POST" => await CreateUser(request),
                "PUT" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await UpdateUser(request, request.PathParameters["userId"]),
                "DELETE" when request.PathParameters != null && request.PathParameters.ContainsKey("userId") => 
                    await DeleteUser(request.PathParameters["userId"]),
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
    
    private async Task<APIGatewayHttpApiV2ProxyResponse> GetUserById(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = JsonSerializer.Serialize(new { Message = $"User with ID {userId} not found" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(user),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
    
    private async Task<APIGatewayHttpApiV2ProxyResponse> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(users),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
    
    private async Task<APIGatewayHttpApiV2ProxyResponse> CreateUser(APIGatewayHttpApiV2ProxyRequest request)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            };
            var user = JsonSerializer.Deserialize<User>(request.Body, options);
            
            if (user == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = JsonSerializer.Serialize(new { Message = "Invalid user data" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            
            var createdUser = await _userRepository.CreateAsync(user);
            
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Created,
                Body = JsonSerializer.Serialize(createdUser, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (JsonException ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(new { Message = $"Invalid JSON format: {ex.Message}" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
    
    private async Task<APIGatewayHttpApiV2ProxyResponse> UpdateUser(APIGatewayHttpApiV2ProxyRequest request, string userId)
    {
        try
        {
            // Check if user exists
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Body = JsonSerializer.Serialize(new { Message = $"User with ID {userId} not found" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            };
            var updatedUser = JsonSerializer.Deserialize<User>(request.Body, options);
            if (updatedUser == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = JsonSerializer.Serialize(new { Message = "Invalid user data" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            
            // Ensure the userId in the path matches the one in the request body
            updatedUser.UserId = userId;
            
            // Preserve creation date
            updatedUser.CreatedAt = existingUser.CreatedAt;
            
            var result = await _userRepository.UpdateAsync(updatedUser);
            
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(result, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (JsonException ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(new { Message = $"Invalid JSON format: {ex.Message}" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
    
    private async Task<APIGatewayHttpApiV2ProxyResponse> DeleteUser(string userId)
    {
        // Check if user exists
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = JsonSerializer.Serialize(new { Message = $"User with ID {userId} not found" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        
        var result = await _userRepository.DeleteAsync(userId);
        
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(new { Success = result, Message = "User deleted successfully" }),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
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
