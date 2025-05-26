using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeuroBrain.Common.Requests
{
    public class CreateNoteRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public string Format { get; set; } = "plain";
        public List<string> Tags { get; set; } = new List<string>();
        public string SourceType { get; set; } = "manual";
        public string SourceUrl { get; set; }
    }

    public class UpdateNoteRequest
    {
        [Required]
        public string NoteId { get; set; }
        
        [StringLength(200)]
        public string Title { get; set; }
        
        public string Content { get; set; }
        public string Format { get; set; }
        public List<string> Tags { get; set; }
    }

    public class GetNotesRequest
    {
        public bool IncludeArchived { get; set; } = false;
        public string Tag { get; set; }
        public string Format { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }

    public class SearchNotesRequest
    {
        [Required]
        public string Query { get; set; }
        
        public bool IncludeContent { get; set; } = true;
        public bool IncludeArchived { get; set; } = false;
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