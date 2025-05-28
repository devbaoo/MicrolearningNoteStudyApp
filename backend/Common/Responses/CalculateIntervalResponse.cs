using System;

namespace Common.Responses
{
    public class CalculateIntervalResponse
    {
        public string AtomId { get; set; }
        public int NewIntervalDays { get; set; }
        public double EaseFactor { get; set; }
        public DateTime NextReviewDate { get; set; }
        public string PerformanceCategory { get; set; }
        public double RetentionProbability { get; set; }
        public double NewDifficultyScore { get; set; }
        public string AlgorithmVersion { get; set; }
        public IntervalCalculationDetails CalculationDetails { get; set; }
    }
}
