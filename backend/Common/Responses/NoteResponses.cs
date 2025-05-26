using System;
using System.Collections.Generic;
using NeuroBrain.Common.Models;

namespace NeuroBrain.Common.Responses
{
    public class NoteResponse
    {
        public string NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Format { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
        public string SourceType { get; set; }
        public string SourceUrl { get; set; }
        public double QualityScore { get; set; }
        public double KnowledgeDensity { get; set; }
        public int WordCount { get; set; }
        public int AtomCount { get; set; }
    }

    public class NoteListResponse
    {
        public List<NoteResponse> Notes { get; set; } = new List<NoteResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class NoteStatsResponse
    {
        public int TotalNotes { get; set; }
        public int ArchivedNotes { get; set; }
        public int TotalWords { get; set; }
        public int TotalAtoms { get; set; }
        public double AverageQualityScore { get; set; }
        public List<string> TopTags { get; set; } = new List<string>();
    }
} 