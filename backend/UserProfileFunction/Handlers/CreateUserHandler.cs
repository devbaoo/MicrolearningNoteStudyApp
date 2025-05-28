using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Common.Requests;
using Common.Responses;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using UserProfileFunction.Services;

namespace UserProfileFunction.Handlers
{
    public class CreateUserHandler : BaseHandler, IHandler
    {
        private readonly IUserService _userService;

        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request)
        {
            try
            {
                var createRequest = DeserializeBody<CreateUserRequest>(request.Body);
                
                if (createRequest == null)
                {
                    return BadRequest("Invalid user data");
                }
                
                // Map request to User model
                var user = new User
                {
                    Email = createRequest.Email,
                    FirstName = createRequest.FirstName,
                    LastName = createRequest.LastName,
                    IsVerified = createRequest.IsVerified,
                    SubscriptionTier = createRequest.SubscriptionTier ?? "free",
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow
                };
                
                // Set subscription expiry if provided
                if (createRequest.SubscriptionExpiry.HasValue)
                {
                    user.SubscriptionExpiry = createRequest.SubscriptionExpiry.Value;
                }
                else
                {
                    // Default expiry depends on subscription tier
                    user.SubscriptionExpiry = createRequest.SubscriptionTier == "free" 
                        ? DateTime.UtcNow.AddYears(100)  // Effectively never expires for free tier
                        : DateTime.UtcNow.AddMonths(1);  // Default 1 month for paid tiers
                }
                
                var createdUser = await _userService.CreateAsync(user);
                
                // Map to response
                var response = new UserResponse
                {
                    UserId = createdUser.UserId,
                    Email = createdUser.Email,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    CreatedAt = createdUser.CreatedAt,
                    LastLogin = createdUser.LastLogin,
                    IsVerified = createdUser.IsVerified,
                    SubscriptionTier = createdUser.SubscriptionTier,
                    SubscriptionExpiry = createdUser.SubscriptionExpiry
                };
                
                return Created(new ApiResponse<UserResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "User created successfully"
                });
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
        }
    }
} 