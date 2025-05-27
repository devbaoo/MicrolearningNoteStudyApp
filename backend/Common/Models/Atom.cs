using System;
using System.Collections.Generic;

namespace Common.Models;

public class Atom
{
    public string AtomId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } = "concept";
    public string? SourceNoteId { get; set; } // Reference to the note this atom belongs to
    public decimal? ImportanceScore { get; set; } = 0.5m;
    public decimal? DifficultyScore { get; set; } = 0.5m;
    public decimal? MasteryLevel { get; set; } = 0;
    public byte[] EmbeddingVector { get; set; } = Array.Empty<byte>();
    public int AccessCount { get; set; } = 0;
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastAccessed { get; set; }
    public bool IsMannuallyCreated { get; set; } = false; // Indicates if the atom was created manually by the user
    //test
}
