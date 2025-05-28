using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Handlers
{
    /// <summary>
    /// Interface for handling review interval calculation requests
    /// </summary>
    public interface ICalculateIntervalHandler
    {
        /// <summary>
        /// Handles the review interval calculation request
        /// </summary>
        /// <param name="request">The API Gateway proxy request containing the interval calculation data</param>
        /// <param name="context">The Lambda execution context</param>
        /// <returns>API Gateway proxy response with the calculated interval or error details</returns>
        Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context);
    }
}
