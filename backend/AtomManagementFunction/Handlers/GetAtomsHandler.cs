using Amazon.Lambda.APIGatewayEvents;
using Common.Responses;
using AtomManagementFunction.Services;
using Newtonsoft.Json;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace AtomManagementFunction.Handlers
{
    public class GetAtomsHandler
    {
        private readonly AtomService _atomService;

        public GetAtomsHandler(AtomService atomService)
        {
            _atomService = atomService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var getRequest = new GetAtomsRequest();

            // Parse query parameters
            if (request.QueryStringParameters != null)
            {
                if (request.QueryStringParameters.ContainsKey("page"))
                {
                    if (int.TryParse(request.QueryStringParameters["page"], out var page))
                    {
                        getRequest.Page = page;
                    }
                    else
                    {
                        getRequest.Page = 1; // Default page if parsing fails
                    }
                }

                if (request.QueryStringParameters.ContainsKey("pageSize"))
                {
                    if (int.TryParse(request.QueryStringParameters["pageSize"], out var pageSize))
                    {
                        getRequest.PageSize = pageSize;
                    }
                    else
                    {
                        getRequest.PageSize = 20; // Default page size if parsing fails
                    }
                }

                if (request.QueryStringParameters.ContainsKey("sortBy"))
                    getRequest.SortBy = request.QueryStringParameters["sortBy"];

                if (request.QueryStringParameters.ContainsKey("sortOrder"))
                    getRequest.SortOrder = request.QueryStringParameters["sortOrder"];

                if (request.QueryStringParameters.ContainsKey("tag"))
                    getRequest.Tag = request.QueryStringParameters["tag"];
            }

            var atoms = await _atomService.GetAtomsAsync(getRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<AtomListResponse>
                {
                    Success = true,
                    Data = atoms,
                    Message = "Atoms retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}