using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class GetNoteTagsHandler
    {
        private readonly NoteService _noteService;

        public GetNoteTagsHandler(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var tags = await _noteService.GetAllTagsAsync(userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = tags,
                    Message = "Tags retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
