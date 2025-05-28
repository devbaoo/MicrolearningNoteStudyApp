using Amazon.Lambda.APIGatewayEvents;
using Common.Responses;
using NeuroBrain.AtomManagementFunction.Services;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using Newtonsoft.Json;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace NeuroBrain.AtomManagementFunction.Handlers
{
    public class SearchAtomsHandler
    {
        private readonly AtomService _atomService;

        public SearchAtomsHandler(AtomService atomService)
        {
            _atomService = atomService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var searchRequest = new SearchAtomsRequest();

            // Parse query parameters
            if (request.QueryStringParameters != null)
            {
                if (request.QueryStringParameters.ContainsKey("query"))
                    searchRequest.Query = request.QueryStringParameters["query"];

                if (request.QueryStringParameters.ContainsKey("includeContent"))
                    bool.TryParse(request.QueryStringParameters["includeContent"], out var includeContent);

                if (request.QueryStringParameters.ContainsKey("page"))
                {
                    if (int.TryParse(request.QueryStringParameters["page"], out var page))
                    {
                        searchRequest.Page = page;
                    }
                    else
                    {
                        searchRequest.Page = 1; // Default page if parsing fails
                    }
                }

                if (request.QueryStringParameters.ContainsKey("pageSize"))
                {
                    if (int.TryParse(request.QueryStringParameters["pageSize"], out var pageSize))
                    {
                        searchRequest.PageSize = pageSize;
                    }
                    else
                    {
                        searchRequest.PageSize = 20; // Default page size if parsing fails
                    }

                    if (request.QueryStringParameters.ContainsKey("tags"))
                    {
                        var tags = request.QueryStringParameters["tags"].Split(',');
                        searchRequest.Tags = tags.ToList();
                    }
                }

                if (string.IsNullOrWhiteSpace(searchRequest.Query))
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        StatusCode = 400,
                        Body = JsonConvert.SerializeObject(new { message = "Search query is required" }),
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }
            }

            var results = await _atomService.SearchAtomsAsync(searchRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<AtomListResponse>
                {
                    Success = true,
                    Data = results,
                    Message = "Search completed successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
