using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Common.Requests;
using Common.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserProfileFunction.Services;

namespace UserProfileFunction.Handlers
{
    public class GetAllUsersHandler : BaseHandler, IHandler
    {
        private readonly IUserService _userService;

        public GetAllUsersHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request)
        {
            // Get query parameters for pagination, sorting, and filtering
            var queryParams = request.QueryStringParameters ?? new Dictionary<string, string>();
            
            var getUsersRequest = new GetUsersRequest
            {
                IncludeUnverified = queryParams.TryGetValue("includeUnverified", out var includeUnverified) && includeUnverified.ToLower() == "true",
                SubscriptionTier = queryParams.TryGetValue("subscriptionTier", out var tier) ? tier : null,
                Page = queryParams.TryGetValue("page", out var page) && int.TryParse(page, out var pageNum) ? pageNum : 1,
                PageSize = queryParams.TryGetValue("pageSize", out var pageSize) && int.TryParse(pageSize, out var pageSizeNum) ? pageSizeNum : 20,
                SortBy = queryParams.TryGetValue("sortBy", out var sortBy) ? sortBy : "CreatedAt",
                SortOrder = queryParams.TryGetValue("sortOrder", out var sortOrder) ? sortOrder : "desc"
            };

            var users = await _userService.GetAllAsync();
            
            // Apply filtering
            if (!getUsersRequest.IncludeUnverified)
            {
                users = users.Where(u => u.IsVerified).ToList();
            }
            
            if (!string.IsNullOrEmpty(getUsersRequest.SubscriptionTier))
            {
                users = users.Where(u => u.SubscriptionTier == getUsersRequest.SubscriptionTier).ToList();
            }

            // Apply sorting
            users = ApplySorting(users, getUsersRequest.SortBy, getUsersRequest.SortOrder);

            // Apply pagination
            var totalCount = users.Count();
            var pagedUsers = users
                .Skip((getUsersRequest.Page - 1) * getUsersRequest.PageSize)
                .Take(getUsersRequest.PageSize)
                .ToList();
            
            // Map to response
            var response = new UserListResponse
            {
                Users = pagedUsers.Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    IsVerified = u.IsVerified,
                    SubscriptionTier = u.SubscriptionTier,
                    SubscriptionExpiry = u.SubscriptionExpiry
                }).ToList(),
                TotalCount = totalCount,
                Page = getUsersRequest.Page,
                PageSize = getUsersRequest.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)getUsersRequest.PageSize)
            };
            
            return Ok(new ApiResponse<UserListResponse>
            {
                Success = true,
                Data = response
            });
        }

        private IEnumerable<User> ApplySorting(IEnumerable<User> users, string sortBy, string sortOrder)
        {
            bool isAscending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);
            
            return sortBy.ToLower() switch
            {
                "email" => isAscending 
                    ? users.OrderBy(u => u.Email) 
                    : users.OrderByDescending(u => u.Email),
                "firstname" => isAscending 
                    ? users.OrderBy(u => u.FirstName) 
                    : users.OrderByDescending(u => u.FirstName),
                "lastname" => isAscending 
                    ? users.OrderBy(u => u.LastName) 
                    : users.OrderByDescending(u => u.LastName),
                "createdat" => isAscending 
                    ? users.OrderBy(u => u.CreatedAt) 
                    : users.OrderByDescending(u => u.CreatedAt),
                "lastlogin" => isAscending 
                    ? users.OrderBy(u => u.LastLogin) 
                    : users.OrderByDescending(u => u.LastLogin),
                "subscriptiontier" => isAscending 
                    ? users.OrderBy(u => u.SubscriptionTier) 
                    : users.OrderByDescending(u => u.SubscriptionTier),
                "subscriptionexpiry" => isAscending 
                    ? users.OrderBy(u => u.SubscriptionExpiry) 
                    : users.OrderByDescending(u => u.SubscriptionExpiry),
                _ => isAscending 
                    ? users.OrderBy(u => u.CreatedAt) 
                    : users.OrderByDescending(u => u.CreatedAt)
            };
        }
    }
} 