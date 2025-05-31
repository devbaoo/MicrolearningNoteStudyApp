using System.Collections.Generic;

namespace Common.Requests
{
    public class StartReviewSessionRequest
    {
        public string UserId { get; set; }
        public string SessionType { get; set; } = "regular";
        public int MaxAtoms { get; set; } = 20;
        public int TimeLimitMinutes { get; set; } = 30;
        public bool? ShuffleOrder { get; set; } = true;
        public bool? ShowHints { get; set; } = false;
    }

    public class SubmitReviewResponseRequest
    {
        public string AtomId { get; set; }
        public ReviewResponseData ResponseData { get; set; }
    }

    public class ReviewResponseData
    {
        public double SuccessRating { get; set; }
        public int ResponseTimeMs { get; set; }
        public double? ConfidenceLevel { get; set; }
        public double? DifficultyPerceived { get; set; }
        public string ReviewMethod { get; set; }
        public string Notes { get; set; }
    }
}
