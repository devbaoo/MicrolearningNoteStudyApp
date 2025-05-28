namespace Common.Responses
{
    public class IntervalCalculationDetails
    {
        public int PreviousInterval { get; set; }
        public double PreviousEaseFactor { get; set; }
        public double SuccessRating { get; set; }
        public int ResponseTimeMs { get; set; }
        public int ReviewCount { get; set; }
        public PerformanceFactors PerformanceFactors { get; set; }
    }
}
