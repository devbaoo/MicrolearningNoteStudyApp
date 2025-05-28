using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using System.Net;
using Common.Models;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Common.Responses;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using ReviewSystemFunction.Handlers;
using ReviewSystemFunction.Services;
using Common.Requests;
using System;
using System.Linq;

namespace ReviewSystemFunction.Tests;

public class FunctionTests : IDisposable
{
    private readonly TestLambdaContext _context;
    private readonly ReviewSystemFunction.Function _function;
    private readonly AmazonDynamoDBClient _dynamoDbClient;
    private readonly CalculateIntervalHandler _calculateIntervalHandler;
    private readonly string _testUserId = "user_456";
    private readonly ISuperMemoService _superMemoService;

    public FunctionTests()
    {
        _context = new TestLambdaContext();
        _dynamoDbClient = new AmazonDynamoDBClient();
        _superMemoService = new SuperMemoService();
        _calculateIntervalHandler = new CalculateIntervalHandler(_dynamoDbClient, _superMemoService);
        
        // Create function instance with real dependencies
        _function = new ReviewSystemFunction.Function();
    }

    public void Dispose()
    {
        _dynamoDbClient?.Dispose();
    }

    private APIGatewayHttpApiV2ProxyRequest CreateCalculateIntervalRequest(ReviewAtom atomData, double successRating, int responseTimeMs, string notes = null)
    {
        var request = new CalculateIntervalRequest
        {
            AtomData = atomData,
            SuccessRating = successRating,
            ResponseTimeMs = responseTimeMs,
            Notes = notes
        };

        return new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "POST",
                    Path = "/reviews/calculate-interval"
                }
            },
            Body = JsonConvert.SerializeObject(request)
        };
    }

    private ReviewAtom CreateTestAtom()
    {
        return new ReviewAtom
        {
            AtomId = "atom_123",
            UserId = _testUserId,
            Content = "Test content",
            Type = "concept",
            ImportanceScore = 0.7m,
            DifficultyScore = 0.5m,
            CurrentInterval = 1,
            EaseFactor = 2.5m,
            ReviewCount = 0,
            NextReviewDate = DateTime.UtcNow.AddDays(1).ToString("o"),
            LastReviewDate = DateTime.UtcNow.AddDays(-1).ToString("o"),
            CreatedAt = DateTime.UtcNow.AddDays(-2).ToString("o"),
            UpdatedAt = DateTime.UtcNow.ToString("o"),
            NoteId = "note_123",
            Tags = new List<string> { "test", "csharp" },
            ReviewSchedule = new ReviewSchedule { IntervalDays = 1 },
            ReviewData = new ReviewData()
        };
    }

    [Fact]
    public async Task CalculateInterval_WithValidRequest_ShouldReturn200()
    {
        // Arrange
        var testAtom = CreateTestAtom();
        var request = CreateCalculateIntervalRequest(testAtom, 0.8, 5000);
        
        // Act
        var response = await _calculateIntervalHandler.HandleAsync(request, _context);

        // Assert
        Assert.Equal(200, response.StatusCode);
        var responseBody = JsonConvert.DeserializeObject<ApiResponse<CalculateIntervalResponse>>(response.Body);
        Assert.True(responseBody.Success);
        Assert.Equal("Review interval calculated successfully", responseBody.Message);
        Assert.NotNull(responseBody.Data);
        Assert.True(responseBody.Data.NewIntervalDays > 0);
        Assert.True(responseBody.Data.EaseFactor > 0);
        Assert.NotNull(responseBody.Data.NextReviewDate);
    }

    [Fact]
    public async Task CalculateInterval_WithInvalidSuccessRating_ShouldReturn400()
    {
        // Arrange
        var testAtom = CreateTestAtom();
        var request = CreateCalculateIntervalRequest(testAtom, 1.5, 5000); // Invalid success rating > 1

        // Act
        var response = await _calculateIntervalHandler.HandleAsync(request, _context);

        // Assert
        Assert.Equal(400, response.StatusCode);
        var responseBody = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.False(responseBody.Success);
        Assert.Contains("Success rating must be between 0 and 1", responseBody.Error);
    }

    [Fact]
    public async Task CalculateInterval_WithNegativeResponseTime_ShouldReturn400()
    {
        // Arrange
        var testAtom = CreateTestAtom();
        var request = CreateCalculateIntervalRequest(testAtom, 0.8, -100); // Negative response time

        // Act
        var response = await _calculateIntervalHandler.HandleAsync(request, _context);

        // Assert
        Assert.Equal(400, response.StatusCode);
        var responseBody = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.False(responseBody.Success);
        Assert.Contains("Response time cannot be negative", responseBody.Error);
    }

    [Fact]
    public async Task CalculateInterval_WithInvalidJson_ShouldReturn400()
    {
        // Arrange
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "POST",
                    Path = "/reviews/calculate-interval"
                }
            },
            Body = "{ invalid json " // Invalid JSON
        };

        // Act
        var response = await _calculateIntervalHandler.HandleAsync(request, _context);

        // Assert
        Assert.Equal(400, response.StatusCode);
        var responseBody = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.False(responseBody.Success);
        Assert.Contains("Invalid JSON format", responseBody.Error);
    }

    [Fact]
    public async Task GetDueReviews_WithValidUserId_ShouldReturn200()
    {
        // Arrange
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "GET",
                    Path = "/reviews/due"
                }
            },
            QueryStringParameters = new Dictionary<string, string>
            {
                { "user_id", _testUserId },
                { "limit", "10" }
            },
        };

        // Act
        var response = await _function.FunctionHandler(request, _context);

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        
        // Parse the response
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        
        // Log the response for debugging
        _context.Logger.LogLine($"Response: {response.Body}");
    }

    [Fact]
    public async Task GetDueReviews_WithMissingUserId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "GET",
                    Path = "/reviews/due"
                }
            },
            QueryStringParameters = new Dictionary<string, string>
            {
                { "limit", "10" }
                // Missing user_id parameter
            }
        };

        // Act
        var response = await _function.FunctionHandler(request, _context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Body);
        
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("user_id", response.Body);
    }

    [Fact]
    public async Task GetDueReviews_WithInvalidLimit_ShouldUseDefaultLimit()
    {
        // Arrange
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "GET",
                    Path = "/reviews/due"
                }
            },
            QueryStringParameters = new Dictionary<string, string>
            {
                { "user_id", _testUserId },
                { "limit", "invalid" }, // Invalid limit
                { "offset", "0" }
            }
        };

        // Act
        var response = await _function.FunctionHandler(request, _context);

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(response.Body);
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
    }

    [Fact]
    public async Task OptionsRequest_ShouldReturnCorsHeaders()
    {
        // Arrange
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "OPTIONS",
                    Path = "/reviews/due"
                }
            }
        };

        // Act
        var response = await _function.FunctionHandler(request, _context);

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.ContainsKey("Access-Control-Allow-Origin"));
        Assert.True(response.Headers.ContainsKey("Access-Control-Allow-Headers"));
        Assert.True(response.Headers.ContainsKey("Access-Control-Allow-Methods"));
    }
}
