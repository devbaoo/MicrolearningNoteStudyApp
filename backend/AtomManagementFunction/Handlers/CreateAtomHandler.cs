using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Common.Responses;
using AtomManagementFunction.Services;
using static Common.Responses.AtomResponses;
using static Common.Requests.AtomRequests;

namespace AtomManagementFunction.Handlers
{
    public class CreateAtomHandler
    {
        private readonly AtomService _atomService;

        public CreateAtomHandler(AtomService atomService)
        {
            _atomService = atomService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var createRequest = JsonConvert.DeserializeObject<CreateAtomRequest>(request.Body);

            if (string.IsNullOrWhiteSpace(createRequest?.Content))
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonConvert.SerializeObject(new { message = "Content are required" }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var atom = await _atomService.CreateAtomAsync(createRequest, userId);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 201,
                Body = JsonConvert.SerializeObject(new ApiResponse<AtomResponse>
                {
                    Success = true,
                    Data = atom,
                    Message = "Atom created successfully"
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
} 