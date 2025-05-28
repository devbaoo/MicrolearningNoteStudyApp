using Common.Models;

namespace Common.Requests
{
    public class CalculateIntervalRequest
    {
        public ReviewAtom AtomData { get; set; }
        public double SuccessRating { get; set; }
        public int ResponseTimeMs { get; set; }
        public string Notes { get; set; }
    }
}
