using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;
using Common.Responses;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class ArchiveNoteHandler
    {
        private readonly NoteService _noteService;

        public ArchiveNoteHandler(NoteService noteService)
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

            var archived = await _noteService.ArchiveNoteAsync(noteId, userId, true);

            if (!archived)
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
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = "Note archived successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private string ExtractNoteIdFromPath(string path)
        {
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
