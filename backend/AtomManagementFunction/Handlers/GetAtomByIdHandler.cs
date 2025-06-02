using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Common.Responses;
using AtomManagementFunction.Services;
using static Common.Responses.AtomResponses;

namespace AtomManagementFunction.Handlers
{
    public class GetAtomByIdHandler
    {
        private readonly AtomService _atomService;

        public GetAtomByIdHandler(AtomService atomService)
        {
            _atomService = atomService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var atomId = ExtractAtomIdFromPath(request.RequestContext.Http.Path);
            
            if (string.IsNullOrEmpty(atomId))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = "Atom ID is required" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var atom = await _atomService.GetAtomByIdAsync(atomId, userId);

            if (atom == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonConvert.SerializeObject(new { message = "Atom not found" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(new ApiResponse<AtomResponse>
                {
                    Success = true,
                    Data = atom,
                    Message = "Atom retrieved successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private string? ExtractAtomIdFromPath(string path)
        {
            // Extract atomId from path like "/atoms/{atomId}"
            var segments = path.Split('/');
            var atomIndex = -1;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "atoms" && i + 1 < segments.Length)
                {
                    atomIndex = i + 1;
                    break;
                }
            }

            return atomIndex >= 0 && atomIndex < segments.Length ? segments[atomIndex] : null;
        }
    }
} 