using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NeuroBrain.Common.Responses;
using Common.Responses;
using NeuroBrain.AtomManagementFunction.Services;

namespace NeuroBrain.AtomManagementFunction.Handlers
{
    public class GetAtomTagsHandler
    {
        private readonly AtomService _atomService;

        public GetAtomTagsHandler(AtomService atomService)
        {
            _atomService = atomService;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, string userId)
        {
            var tags = await _atomService.GetAllTagsAsync(userId);

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
