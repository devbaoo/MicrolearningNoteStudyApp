# DeleteNoteHandler.cs
$deleteHandlerContent = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class DeleteNoteHandler
    {
        private readonly NoteService _noteService;

        public DeleteNoteHandler(NoteService noteService)
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
                    Body = JsonConvert.SerializeObject(new { message = `"Note ID is required`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            var deleted = await _noteService.DeleteNoteAsync(noteId, userId);

            if (!deleted)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonConvert.SerializeObject(new { message = `"Note not found`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = `"Note deleted successfully`"
                }),
                Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
            };
        }

        private string ExtractNoteIdFromPath(string path)
        {
            var segments = path.Split('/');
            var noteIndex = -1;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == `"notes`" && i + 1 < segments.Length)
                {
                    noteIndex = i + 1;
                    break;
                }
            }

            return noteIndex >= 0 && noteIndex < segments.Length ? segments[noteIndex] : null;
        }
    }
}
"@

# SearchNotesHandler.cs
$searchHandlerContent = @"
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class SearchNotesHandler
    {
        private readonly NoteService _noteService;

        public SearchNotesHandler(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var searchRequest = new SearchNotesRequest();

            // Parse query parameters
            if (request.QueryStringParameters != null)
            {
                if (request.QueryStringParameters.ContainsKey(`"query`"))
                    searchRequest.Query = request.QueryStringParameters[`"query`"];

                if (request.QueryStringParameters.ContainsKey(`"includeContent`"))
                    bool.TryParse(request.QueryStringParameters[`"includeContent`"], out var includeContent);

                if (request.QueryStringParameters.ContainsKey(`"includeArchived`"))
                    bool.TryParse(request.QueryStringParameters[`"includeArchived`"], out var includeArchived);

                if (request.QueryStringParameters.ContainsKey(`"page`"))
                    int.TryParse(request.QueryStringParameters[`"page`"], out searchRequest.Page);

                if (request.QueryStringParameters.ContainsKey(`"pageSize`"))
                    int.TryParse(request.QueryStringParameters[`"pageSize`"], out searchRequest.PageSize);

                if (request.QueryStringParameters.ContainsKey(`"tags`"))
                {
                    var tags = request.QueryStringParameters[`"tags`"].Split(',');
                    searchRequest.Tags = tags.ToList();
                }
            }

            if (string.IsNullOrWhiteSpace(searchRequest.Query))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = `"Search query is required`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            var results = await _noteService.SearchNotesAsync(searchRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<NoteListResponse>
                {
                    Success = true,
                    Data = results,
                    Message = `"Search completed successfully`"
                }),
                Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
            };
        }
    }
}
"@

# ArchiveNoteHandler.cs
$archiveHandlerContent = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

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
                    Body = JsonConvert.SerializeObject(new { message = `"Note ID is required`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            var archived = await _noteService.ArchiveNoteAsync(noteId, userId, true);

            if (!archived)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonConvert.SerializeObject(new { message = `"Note not found`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = `"Note archived successfully`"
                }),
                Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
            };
        }

        private string ExtractNoteIdFromPath(string path)
        {
            var segments = path.Split('/');
            var noteIndex = -1;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == `"notes`" && i + 1 < segments.Length)
                {
                    noteIndex = i + 1;
                    break;
                }
            }

            return noteIndex >= 0 && noteIndex < segments.Length ? segments[noteIndex] : null;
        }
    }
}
"@

# RestoreNoteHandler.cs
$restoreHandlerContent = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using NeuroBrain.NoteManagementFunction.Services;

namespace NeuroBrain.NoteManagementFunction.Handlers
{
    public class RestoreNoteHandler
    {
        private readonly NoteService _noteService;

        public RestoreNoteHandler(NoteService noteService)
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
                    Body = JsonConvert.SerializeObject(new { message = `"Note ID is required`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            var restored = await _noteService.ArchiveNoteAsync(noteId, userId, false);

            if (!restored)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonConvert.SerializeObject(new { message = `"Note not found`" }),
                    Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
                };
            }

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = `"Note restored successfully`"
                }),
                Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
            };
        }

        private string ExtractNoteIdFromPath(string path)
        {
            var segments = path.Split('/');
            var noteIndex = -1;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == `"notes`" && i + 1 < segments.Length)
                {
                    noteIndex = i + 1;
                    break;
                }
            }

            return noteIndex >= 0 && noteIndex < segments.Length ? segments[noteIndex] : null;
        }
    }
}
"@

# GetNoteTagsHandler.cs
$tagsHandlerContent = @"
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
                    Message = `"Tags retrieved successfully`"
                }),
                Headers = new Dictionary<string, string> { { `"Content-Type`", `"application/json`" } }
            };
        }
    }
}
"@

# Replace backticks with actual double quotes
$deleteHandlerContent = $deleteHandlerContent -replace '\`"', '"'
$searchHandlerContent = $searchHandlerContent -replace '\`"', '"'
$archiveHandlerContent = $archiveHandlerContent -replace '\`"', '"'
$restoreHandlerContent = $restoreHandlerContent -replace '\`"', '"'
$tagsHandlerContent = $tagsHandlerContent -replace '\`"', '"'

# Write to files
$deleteHandlerContent | Out-File -Encoding utf8 -FilePath "backend/NoteManagementFunction/Handlers/DeleteNoteHandler.cs"
$searchHandlerContent | Out-File -Encoding utf8 -FilePath "backend/NoteManagementFunction/Handlers/SearchNotesHandler.cs"
$archiveHandlerContent | Out-File -Encoding utf8 -FilePath "backend/NoteManagementFunction/Handlers/ArchiveNoteHandler.cs"
$restoreHandlerContent | Out-File -Encoding utf8 -FilePath "backend/NoteManagementFunction/Handlers/RestoreNoteHandler.cs"
$tagsHandlerContent | Out-File -Encoding utf8 -FilePath "backend/NoteManagementFunction/Handlers/GetNoteTagsHandler.cs"

Write-Host "Created all handler files successfully!" 