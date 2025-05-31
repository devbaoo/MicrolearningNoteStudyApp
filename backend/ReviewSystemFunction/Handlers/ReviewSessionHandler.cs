using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Requests;
using Common.Responses;
using Newtonsoft.Json;
using ReviewSystemFunction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Handlers
{
    public class ReviewSessionHandler
    {
        private readonly IReviewSessionService _reviewSessionService;

        public ReviewSessionHandler(IReviewSessionService reviewSessionService)
        {
            _reviewSessionService = reviewSessionService ?? throw new ArgumentNullException(nameof(reviewSessionService));
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Get the path and method, ensuring path is normalized to lowercase for consistent matching
                var path = request.RequestContext.Http.Path?.ToLower() ?? "";
                var method = request.RequestContext.Http.Method?.ToUpper() ?? "";

                context.Logger.LogInformation($"Processing {method} {path}");

                // Route to appropriate action
                if (method == "POST" && path == "/reviews/sessions")
                {
                    return await StartSessionAsync(request, context);
                }
                else if (method == "POST" && path.Contains("/reviews/sessions/") && path.EndsWith("/responses"))
                {
                    return await SubmitResponseAsync(request, context);
                }
                else if (method == "PUT" && path.Contains("/reviews/sessions/") && path.EndsWith("/end"))
                {
                    return await EndSessionAsync(request, context);
                }
                else if (method == "GET" && path.Contains("/reviews/sessions/") && !path.EndsWith("/responses") && !path.EndsWith("/end"))
                {
                    return await GetSessionAsync(request, context);
                }
                else
                {
                    return CreateErrorResponse(404, "Endpoint not found");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error in ReviewSessionHandler: {ex.Message}");
                return CreateErrorResponse(500, "Internal server error");
            }
        }

        #region Start Review Session
        
        private async Task<APIGatewayHttpApiV2ProxyResponse> StartSessionAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Starting new review session");

                // Parse request
                if (string.IsNullOrEmpty(request.Body))
                {
                    return CreateErrorResponse(400, "Request body is required");
                }

                StartReviewSessionRequest startRequest;
                try
                {
                    startRequest = JsonConvert.DeserializeObject<StartReviewSessionRequest>(request.Body);
                }
                catch (JsonException ex)
                {
                    context.Logger.LogError($"Invalid JSON in request body: {ex.Message}");
                    return CreateErrorResponse(400, "Invalid JSON format in request body");
                }
                
                // Validate request
                if (string.IsNullOrEmpty(startRequest?.UserId))
                {
                    return CreateErrorResponse(400, "UserId is required");
                }

                // Delegate to service layer
                var response = await _reviewSessionService.StartSessionAsync(startRequest, context);

                return CreateSuccessResponse(response);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error starting session: {ex.Message}");
                return CreateErrorResponse(500, "Failed to start review session");
            }
        }

        #endregion

        #region Submit Review Response

        private async Task<APIGatewayHttpApiV2ProxyResponse> SubmitResponseAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Extract session ID from path
                var sessionId = ExtractSessionIdFromPath(request.RequestContext.Http.Path);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return CreateErrorResponse(400, "Invalid session ID in path");
                }

                context.Logger.LogInformation($"Submitting response for session: {sessionId}");

                // Parse request
                if (string.IsNullOrEmpty(request.Body))
                {
                    return CreateErrorResponse(400, "Request body is required");
                }

                SubmitReviewResponseRequest submitRequest;
                try
                {
                    submitRequest = JsonConvert.DeserializeObject<SubmitReviewResponseRequest>(request.Body);
                }
                catch (JsonException ex)
                {
                    context.Logger.LogError($"Invalid JSON in request body: {ex.Message}");
                    return CreateErrorResponse(400, "Invalid JSON format in request body");
                }

                // Delegate to service layer
                try
                {
                    var response = await _reviewSessionService.SubmitResponseAsync(sessionId, submitRequest, context);
                    return CreateSuccessResponse(response);
                }
                catch (ArgumentException ex)
                {
                    return CreateErrorResponse(400, ex.Message);
                }
                catch (KeyNotFoundException ex)
                {
                    return CreateErrorResponse(404, ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return CreateErrorResponse(400, ex.Message);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error submitting response: {ex.Message}");
                return CreateErrorResponse(500, "Failed to record review response");
            }
        }

        #endregion

        #region End Review Session

        private async Task<APIGatewayHttpApiV2ProxyResponse> EndSessionAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var sessionId = ExtractSessionIdFromPath(request.RequestContext.Http.Path);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return CreateErrorResponse(400, "Invalid session ID in path");
                }

                context.Logger.LogInformation($"Ending session: {sessionId}");

                try
                {
                    // Delegate to service layer
                    var response = await _reviewSessionService.EndSessionAsync(sessionId, context);
                    return CreateSuccessResponse(response);
                }
                catch (KeyNotFoundException ex)
                {
                    return CreateErrorResponse(404, ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return CreateErrorResponse(400, ex.Message);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error ending session: {ex.Message}");
                return CreateErrorResponse(500, "Failed to end review session");
            }
        }

        #endregion

        #region Get Session Status

        private async Task<APIGatewayHttpApiV2ProxyResponse> GetSessionAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var sessionId = ExtractSessionIdFromPath(request.RequestContext.Http.Path);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return CreateErrorResponse(400, "Invalid session ID in path");
                }

                try
                {
                    // Delegate to service layer
                    var response = await _reviewSessionService.GetSessionAsync(sessionId, context);
                    return CreateSuccessResponse(response);
                }
                catch (KeyNotFoundException ex)
                {
                    return CreateErrorResponse(404, ex.Message);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error getting session: {ex.Message}");
                return CreateErrorResponse(500, "Failed to get session data");
            }
        }

        #endregion

        #region Helper Methods

        private string ExtractSessionIdFromPath(string path)
        {
            // Extract session ID from paths like /reviews/sessions/{sessionId}/responses or /reviews/sessions/{sessionId}/end
            var parts = path?.Split('/');
            if (parts?.Length >= 4 && parts[1] == "reviews" && parts[2] == "sessions")
            {
                return parts[3];
            }
            return null;
        }

        private APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse(object data)
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
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = data,
                    Message = "Operation completed successfully"
                })
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
                })
            };
        }

        #endregion
    }
}
