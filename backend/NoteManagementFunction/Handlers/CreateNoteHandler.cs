using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class CreateNoteHandler
    {
        private readonly NoteService _noteService;

        public CreateNoteHandler(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var createRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(request.Body);
            
            if (string.IsNullOrWhiteSpace(createRequest.Title) || string.IsNullOrWhiteSpace(createRequest.Content))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = "Title and Content are required" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var note = await _noteService.CreateNoteAsync(createRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 201,
                Body = JsonConvert.SerializeObject(new ApiResponse<NoteResponse>
                {
                    Success = true,
                    Data = note,
                    Message = "Note created successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
} 