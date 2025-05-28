using Amazon.Lambda.APIGatewayEvents;
using Common.Responses;
using System.Threading.Tasks;
using UserProfileFunction.Services;

namespace UserProfileFunction.Handlers
{
    public class GetUserByIdHandler : BaseHandler, IHandler
    {
        private readonly IUserService _userService;

        public GetUserByIdHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request)
        {
            if (!request.PathParameters.TryGetValue("userId", out string userId))
            {
                return BadRequest("User ID is required");
            }

            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }
            
            // Map to response
            var response = new UserResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                IsVerified = user.IsVerified,
                SubscriptionTier = user.SubscriptionTier,
                SubscriptionExpiry = user.SubscriptionExpiry
            };
            
            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Data = response
            });
        }
    }
} 