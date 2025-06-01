using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Models;
using Common.Requests;
using Common.Responses;
using Newtonsoft.Json;
using ReviewSystemFunction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Handlers
//test
{
    public class CalculateIntervalHandler : ICalculateIntervalHandler
    {
        private readonly ISuperMemoService _superMemoService;
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public CalculateIntervalHandler() : this(new AmazonDynamoDBClient(), new SuperMemoService())
        {
        }

        public CalculateIntervalHandler(IAmazonDynamoDB dynamoDbClient, ISuperMemoService superMemoService)
        {
            _dynamoDbClient = dynamoDbClient ?? throw new ArgumentNullException(nameof(dynamoDbClient));
            _superMemoService = superMemoService ?? throw new ArgumentNullException(nameof(superMemoService));
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation($"Starting CalculateInterval handler. Request: {JsonConvert.SerializeObject(request)}");

                // Parse request body
                if (string.IsNullOrEmpty(request.Body))
                {
                    context.Logger.LogError("Empty request body received");
                    return CreateErrorResponse(400, "Request body is required");
                }
                
                context.Logger.LogInformation($"Request body: {request.Body}");

                CalculateIntervalRequest calculateRequest;
                try 
                {
                    calculateRequest = JsonConvert.DeserializeObject<CalculateIntervalRequest>(request.Body);
                    if (calculateRequest == null)
                    {
                        context.Logger.LogError("Failed to deserialize request body");
                        return CreateErrorResponse(400, "Invalid request format");
                    }
                    
                    // Validate request
                    var validationResult = ValidateRequest(calculateRequest);
                    if (!validationResult.IsValid)
                    {
                        context.Logger.LogError($"Request validation failed: {validationResult.ErrorMessage}");
                        return CreateErrorResponse(400, validationResult.ErrorMessage);
                    }
                }
                catch (JsonException ex)
                {
                    context.Logger.LogError($"JSON deserialization error: {ex.Message}");
                    return CreateErrorResponse(400, $"Invalid JSON: {ex.Message}");
                }

                if (calculateRequest.AtomData == null)
                {
                    return CreateErrorResponse(400, "Atom data is required");
                }

                context.Logger.LogInformation($"Calculating interval for atom: {calculateRequest.AtomData.AtomId}");

                // Calculate next review interval using SuperMemo-2 algorithm
                var result = await _superMemoService.CalculateNextReviewIntervalAsync(
                    calculateRequest.AtomData, 
                    calculateRequest.SuccessRating,
                    calculateRequest.ResponseTimeMs,
                    context
                );

                context.Logger.LogInformation($"Calculated new interval: {result.NewIntervalDays} days");

                // Update the atom in DynamoDB with the new review schedule
                // This would be implemented in a separate service/repository
                // await _reviewRepository.UpdateReviewScheduleAsync(calculateRequest.AtomData.AtomId, result);

                return CreateSuccessResponse(result);
            }
            catch (JsonException ex)
            {
                context.Logger.LogError($"Invalid JSON in request body: {ex.Message}");
                return CreateErrorResponse(400, "Invalid JSON format in request body");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error calculating review interval: {ex}");
                return CreateErrorResponse(500, "Failed to calculate review interval");
            }
        }

        private (bool IsValid, string ErrorMessage) ValidateRequest(CalculateIntervalRequest request)
        {
            if (request == null)
                return (false, "Request object is required");

            if (request.AtomData == null)
                return (false, "Atom data is required");

if (string.IsNullOrEmpty(request.AtomData.AtomId))
                return (false, "Atom ID is required");

            if (request.SuccessRating < 0 || request.SuccessRating > 1)
                return (false, "Success rating must be between 0 and 1");

            if (request.ResponseTimeMs < 0)
                return (false, "Response time cannot be negative");

            return (true, string.Empty);
        }

        private APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse(CalculateIntervalResponse data)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*",
                    ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
                    ["Access-Control-Allow-Headers"] = "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"
                },
                Body = JsonConvert.SerializeObject(new ApiResponse<CalculateIntervalResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Review interval calculated successfully"
                }),
                IsBase64Encoded = false
            };
        }

        private APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(int statusCode, string message)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Access-Control-Allow-Origin"] = "*",
                    ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
                    ["Access-Control-Allow-Headers"] = "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"
                },
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = false,
                    Error = message,
                    Timestamp = DateTime.UtcNow
                }),
                IsBase64Encoded = false
            };
        }
    }
}
