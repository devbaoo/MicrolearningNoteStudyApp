using System.ComponentModel.DataAnnotations;

namespace Common.Requests;

public class AtomRequests
{
    public class CreateAtomRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string Type { get; set; } = "concept";
        public decimal? ImportanceScore { get; set; } = 0.5m;
        public decimal? DifficultyScore { get; set; } = 0.5m;
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public string? SourceNoteId { get; set; }
        public byte[]? EmbeddingVector { get; set; } = null;
        public bool IsMannuallyCreated { get; set; } = false;
        public decimal? MasteryLevel { get; set; } = 0;
        public int AccessCount { get; set; } = 0;
    }

    public class UpdateAtomRequest
    {
        [Required]
        public string AtomId { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }

        public string? SourceNoteId { get; set; }
        public decimal? ImportanceScore { get; set; } = 0.5m;
        public decimal? DifficultyScore { get; set; } = 0.5m;
        public decimal? MasteryLevel { get; set; } = 0;
        public byte[] EmbeddingVector { get; set; } = Array.Empty<byte>();
        public List<string> Tags { get; set; }
    }

    public class GetAtomsRequest
    {
        public bool IncludeArchived { get; set; } = false;
        public string Tag { get; set; }
        public string Format { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }

    public class SearchAtomsRequest
    {
        [Required]
        public string Query { get; set; }

        public bool IncludeContent { get; set; } = true;
        public List<string> Tags { get; set; } = new List<string>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ArchiveNoteRequest
    {
        [Required]
        public string NoteId { get; set; }

        public bool Archive { get; set; } = true;
    }
} 