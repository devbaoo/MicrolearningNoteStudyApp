using System;
using System.Collections.Generic;

namespace NeuroBrain.Common.Models
{
    public class Note
    {
        public string NoteId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Format { get; set; } // cornell, zettelkasten, mindmap, plain
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false;
        public string SourceType { get; set; } // manual, imported, web, pdf
        public string SourceUrl { get; set; }
        public double QualityScore { get; set; }
        public double KnowledgeDensity { get; set; }
        public int WordCount { get; set; }
        public int AtomCount { get; set; }
    }
} 