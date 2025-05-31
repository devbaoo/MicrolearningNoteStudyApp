using Amazon.Lambda.Core;
using Common.Models;
using Common.Requests;
using Common.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public interface IReviewSessionService
    {
        // Start a new review session
        Task<StartReviewSessionResponse> StartSessionAsync(StartReviewSessionRequest request, ILambdaContext context);
        
        // Submit a review response
        Task<SubmitReviewResponseResponse> SubmitResponseAsync(string sessionId, SubmitReviewResponseRequest request, ILambdaContext context);
        
        // End a review session
        Task<EndReviewSessionResponse> EndSessionAsync(string sessionId, ILambdaContext context);
        
        // Get session status
        Task<GetSessionResponse> GetSessionAsync(string sessionId, ILambdaContext context);
        
        // Supporting methods
        Task<ReviewSession> GetSessionByIdAsync(string sessionId, ILambdaContext context);
        Task<ReviewAtom> GetAtomDataAsync(string atomId, ILambdaContext context);
        Task<string> StoreReviewResponseAsync(string sessionId, SubmitReviewResponseRequest request, CalculateIntervalResponse intervalResult, ILambdaContext context);
        Task UpdateAtomSchedulingAsync(string atomId, CalculateIntervalResponse intervalResult, ILambdaContext context);
        Task<ReviewSession> UpdateSessionProgressAsync(string sessionId, string atomId, ILambdaContext context);
        Task UpdateSessionStatusAsync(string sessionId, string status, ILambdaContext context);
        Task<SessionStatistics> CalculateSessionStatisticsAsync(string sessionId, ILambdaContext context);
        int CalculateEstimatedTime(List<ReviewAtomForSession> atoms);
        List<string> GenerateImprovementSuggestions(CalculateIntervalResponse intervalResult);
        PerformanceSummary GeneratePerformanceSummary(SessionStatistics stats);
        Task<string> GenerateNextReviewSuggestionAsync(string userId, ILambdaContext context);
    }
}
