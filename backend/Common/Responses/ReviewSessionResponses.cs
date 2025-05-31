using Common.Models;
using System;
using System.Collections.Generic;

namespace Common.Responses
{
    public class StartReviewSessionResponse
    {
        public string SessionId { get; set; }
        public List<ReviewAtomForSession> AtomsToReview { get; set; } = new List<ReviewAtomForSession>();
        public int TotalAtoms { get; set; }
        public int EstimatedTimeMinutes { get; set; }
        public SessionSettings SessionSettings { get; set; }
        public string Message { get; set; }
    }

    public class ReviewAtomForSession
    {
        public string AtomId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public decimal ImportanceScore { get; set; }
        public int ReviewCount { get; set; }
    }

    public class SubmitReviewResponseResponse
    {
        public string ResponseId { get; set; }
        public string AtomId { get; set; }
        public DateTime NextReviewDate { get; set; }
        public int NewIntervalDays { get; set; }
        public string PerformanceCategory { get; set; }
        public double RetentionProbability { get; set; }
        public List<string> ImprovementSuggestions { get; set; } = new List<string>();
        public SessionProgress SessionProgress { get; set; }
    }

    public class SessionProgress
    {
        public string SessionId { get; set; }
        public int TotalAtoms { get; set; }
        public int CompletedAtoms { get; set; }
        public int RemainingAtoms { get; set; }
        public double ProgressPercentage { get; set; }
        public bool IsComplete { get; set; }
    }

    public class EndReviewSessionResponse
    {
        public string SessionId { get; set; }
        public DateTime CompletedAt { get; set; }
        public int SessionDurationMinutes { get; set; }
        public SessionStatistics SessionStatistics { get; set; }
        public PerformanceSummary PerformanceSummary { get; set; }
        public string NextReviewSuggestion { get; set; }
    }

    public class GetSessionResponse
    {
        public string SessionId { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public int TotalAtoms { get; set; }
        public int CompletedAtoms { get; set; }
        public int RemainingAtoms { get; set; }
        public double ProgressPercentage { get; set; }
        public SessionSettings SessionSettings { get; set; }
    }

    public class SessionStatistics
    {
        public int TotalResponses { get; set; }
        public double AverageSuccessRating { get; set; }
        public int AverageResponseTimeMs { get; set; }
        public int ExcellentCount { get; set; }
        public int GoodCount { get; set; }
        public int FairCount { get; set; }
        public int NeedsReviewCount { get; set; }
        public int FastestResponseMs { get; set; }
        public int SlowestResponseMs { get; set; }
    }

    public class PerformanceSummary
    {
        public string OverallGrade { get; set; }
        public List<string> StrengthAreas { get; set; } = new List<string>();
        public List<string> ImprovementAreas { get; set; } = new List<string>();
        public List<string> StudyRecommendations { get; set; } = new List<string>();
    }
}
