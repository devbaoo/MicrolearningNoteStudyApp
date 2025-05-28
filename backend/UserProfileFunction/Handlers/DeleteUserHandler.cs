using Amazon.Lambda.APIGatewayEvents;
using Common.Responses;
using System.Threading.Tasks;
using UserProfileFunction.Services;

namespace UserProfileFunction.Handlers
{
    public class DeleteUserHandler : BaseHandler, IHandler
    {
        private readonly IUserService _userService;

        public DeleteUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request)
        {
            if (!request.PathParameters.TryGetValue("userId", out string userId))
            {
                return BadRequest("User ID is required");
            }

            // Check if user exists
            var existingUser = await _userService.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {userId} not found");
            }
            
            var result = await _userService.DeleteAsync(userId);
            
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = result,
                Message = "User deleted successfully"
            });
        }
    }
} 