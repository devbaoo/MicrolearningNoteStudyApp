using Amazon.Lambda.APIGatewayEvents;
using Common.Responses;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;
using Newtonsoft.Json;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class GetNotesHandler
    {
        private readonly NoteService _noteService;

        public GetNotesHandler(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var getRequest = new GetNotesRequest();

            // Parse query parameters
            if (request.QueryStringParameters != null)
            {
                if (request.QueryStringParameters.ContainsKey("includeArchived"))
                    bool.TryParse(request.QueryStringParameters["includeArchived"], out var includeArchived);

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

            var notes = await _noteService.GetNotesAsync(getRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<NoteListResponse>
                {
                    Success = true,
                    Data = notes,
                    Message = "Notes retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}