using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Common.Responses;
using NeuroBrain.AtomManagementFunction.Services;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace NeuroBrain.AtomManagementFunction.Handlers
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
                    int.TryParse(request.QueryStringParameters["page"], out getRequest.Page);

                if (request.QueryStringParameters.ContainsKey("pageSize"))
                    int.TryParse(request.QueryStringParameters["pageSize"], out getRequest.PageSize);

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