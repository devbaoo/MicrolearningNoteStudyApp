using Common.Models;
using System.Collections.Generic;

namespace Common.Responses
{
    public class GetDueReviewsResponse
    {
        public List<ReviewAtom> DueAtoms { get; set; } = new List<ReviewAtom>();
        public int TotalCount { get; set; }
        public bool ReviewLimitReached { get; set; }
        public int EstimatedReviewTimeMinutes { get; set; }
        public string NextReviewTime { get; set; }
    }
}