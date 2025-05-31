using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using Common.Responses;
using NeuroBrain.AtomManagementFunction.Services;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace NeuroBrain.AtomManagementFunction.Handlers
{
    public class UpdateAtomTagsHandler
    {
        private readonly AtomService _atomService;

        public UpdateAtomTagsHandler(AtomService atomService)
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

            var updateRequest = JsonConvert.DeserializeObject<UpdateAtomTagsRequest>(request.Body);
            
            if (updateRequest?.Tags == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = "Tags are required" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var atom = await _atomService.UpdateAtomTagsAsync(atomId, updateRequest, userId);

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
                    Message = "Atom tags updated successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private string ExtractAtomIdFromPath(string path)
        {
            var segments = path.Split('/');
            var atomIdIndex = Array.IndexOf(segments, "atoms") + 1;
            return atomIdIndex < segments.Length ? segments[atomIdIndex] : null;
        }
    }
} 