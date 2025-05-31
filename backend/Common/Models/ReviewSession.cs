using System.Collections.Generic;

namespace Common.Models
{
    public class ReviewSession
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string SessionType { get; set; } = "regular";
        public string Status { get; set; } = "active";
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalAtoms { get; set; }
        public int CompletedAtoms { get; set; }
        public List<string> AtomsToReview { get; set; } = new List<string>();
        public SessionSettings SessionSettings { get; set; } = new SessionSettings();
    }
}