using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class GetNoteByIdHandler
    {
        private readonly NoteService _noteService;

        public GetNoteByIdHandler(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var noteId = ExtractNoteIdFromPath(request.RequestContext.Http.Path);
            
            if (string.IsNullOrEmpty(noteId))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = "Note ID is required" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var note = await _noteService.GetNoteByIdAsync(noteId, userId);

            if (note == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonConvert.SerializeObject(new { message = "Note not found" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<NoteResponse>
                {
                    Success = true,
                    Data = note,
                    Message = "Note retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private string ExtractNoteIdFromPath(string path)
        {
            // Extract noteId from path like "/notes/{noteId}"
            var segments = path.Split('/');
            var noteIndex = -1;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "notes" && i + 1 < segments.Length)
                {
                    noteIndex = i + 1;
                    break;
                }
            }

            return noteIndex >= 0 && noteIndex < segments.Length ? segments[noteIndex] : null;
        }
    }
} 