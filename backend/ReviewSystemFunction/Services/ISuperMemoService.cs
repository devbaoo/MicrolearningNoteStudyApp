using Amazon.Lambda.Core;
using Common.Models;
using Common.Responses;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public interface ISuperMemoService
    {
        Task<CalculateIntervalResponse> CalculateNextReviewIntervalAsync(
            ReviewAtom atomData,
            double successRating,
            int responseTimeMs,
            ILambdaContext context);
    }
}
