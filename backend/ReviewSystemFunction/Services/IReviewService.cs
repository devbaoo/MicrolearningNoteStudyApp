using Common.Models;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public interface IReviewService
    {
        // Main orchestration method that combines all business logic
        Task<ReviewData> GetDueReviewsDataAsync(string userId, int limit, ILambdaContext context);
        
        // Supporting methods used by the main method
        //test
        Task<List<ReviewAtom>> GetDueAtomsAsync(string userId, int limit, ILambdaContext context);
        double CalculatePriority(ReviewAtom atom);
        int CalculateEstimatedTime(List<ReviewAtom> atoms);
        string GetNextReviewTime(List<ReviewAtom> atoms);
    }
}
