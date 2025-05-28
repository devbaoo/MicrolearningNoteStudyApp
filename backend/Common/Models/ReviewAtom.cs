using System.Collections.Generic;

namespace Common.Models;

public class ReviewAtom
{
    // Alias for AtomId to match SuperMemoService expectations
    public string Id => AtomId;
    
    public required string AtomId { get; set; }
    public required string UserId { get; set; }
    public required string Content { get; set; }
    public string Type { get; set; } = "concept";
    public decimal? ImportanceScore { get; set; } = 0.5m;
    public decimal? DifficultyScore { get; set; } = 0.5m;
    public int CurrentInterval { get; set; } = 1;
    public decimal EaseFactor { get; set; } = 2.5m;
    public int ReviewCount { get; set; } = 0;
    public required string NextReviewDate { get; set; }
    public required string LastReviewDate { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public required string CreatedAt { get; set; }
    public required string UpdatedAt { get; set; }
    public required string NoteId { get; set; }
    
    // Add the missing properties
    public ReviewSchedule ReviewSchedule { get; set; } = new();
    public ReviewData ReviewData { get; set; } = new();
}
