using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;

namespace UserProfileFunction.Handlers
{
    public interface IHandler
    {
        Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request);
    }
} 