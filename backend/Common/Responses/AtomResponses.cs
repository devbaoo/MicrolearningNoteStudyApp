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
        public string? SourceNoteId { get; set; }
        public decimal? ImportanceScore { get; set; }
        public decimal? DifficultyScore { get; set; }
        public decimal? MasteryLevel { get; set; }
        public byte[] EmbeddingVector { get; set; }
        public int AccessCount { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool IsMannuallyCreated { get; set; }
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