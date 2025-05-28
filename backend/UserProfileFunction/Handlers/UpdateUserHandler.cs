using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Common.Requests;
using Common.Responses;
using System.Text.Json;
using System.Threading.Tasks;
using UserProfileFunction.Services;

namespace UserProfileFunction.Handlers
{
    public class UpdateUserHandler : BaseHandler, IHandler
    {
        private readonly IUserService _userService;

        public UpdateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request)
        {
            try
            {
                if (!request.PathParameters.TryGetValue("userId", out string userId))
                {
                    return BadRequest("User ID is required");
                }

                var updateRequest = DeserializeBody<UpdateUserRequest>(request.Body);
                if (updateRequest == null)
                {
                    return BadRequest("Invalid user data");
                }
                
                // Get the existing user first
                var existingUser = await _userService.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                // Update only the fields that were provided
                if (updateRequest.Email != null)
                    existingUser.Email = updateRequest.Email;
                
                if (updateRequest.FirstName != null)
                    existingUser.FirstName = updateRequest.FirstName;
                
                if (updateRequest.LastName != null)
                    existingUser.LastName = updateRequest.LastName;
                
                if (updateRequest.IsVerified.HasValue)
                    existingUser.IsVerified = updateRequest.IsVerified.Value;
                
                if (updateRequest.SubscriptionTier != null)
                    existingUser.SubscriptionTier = updateRequest.SubscriptionTier;
                
                if (updateRequest.SubscriptionExpiry.HasValue)
                    existingUser.SubscriptionExpiry = updateRequest.SubscriptionExpiry.Value;
                
                if (updateRequest.LastLogin.HasValue)
                    existingUser.LastLogin = updateRequest.LastLogin.Value;
                
                var result = await _userService.UpdateAsync(existingUser, userId);
                
                if (result == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                
                // Map to response
                var response = new UserResponse
                {
                    UserId = result.UserId,
                    Email = result.Email,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    CreatedAt = result.CreatedAt,
                    LastLogin = result.LastLogin,
                    IsVerified = result.IsVerified,
                    SubscriptionTier = result.SubscriptionTier,
                    SubscriptionExpiry = result.SubscriptionExpiry
                };
                
                return Ok(new ApiResponse<UserResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "User updated successfully"
                });
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
        }
    }
} 