using NeuroBrain.Common.Responses;

namespace Common.Responses;

public class AtomResponses
{
    public class AtomResponse
    {
        public string AtomId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public decimal? ImportanceScore { get; set; }
        public decimal? DifficultyScore { get; set; }
        public int CurrentInterval { get; set; }
        public decimal EaseFactor { get; set; }
        public int ReviewCount { get; set; } = 0;
        public string NextReviewDate { get; set; }
        public string LastReviewDate { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AtomListResponse
    {
        public List<AtomResponse> Atoms { get; set; } = new List<AtomResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
} 