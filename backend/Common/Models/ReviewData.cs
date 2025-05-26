using System.Collections.Generic;

namespace Common.Models
{
    public class ReviewData
    {
        public List<ReviewAtom> SortedAtoms { get; set; }
        public int EstimatedTimeMinutes { get; set; }
        public string NextReviewTime { get; set; }
    }
}
