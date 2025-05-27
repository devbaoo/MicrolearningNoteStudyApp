using System;
using System.Collections.Generic;

namespace Common.Models;

public class Atom
{
    public string AtomId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } = "concept";
    public decimal? ImportanceScore { get; set; } = 0.5m;
    public decimal? DifficultyScore { get; set; } = 0.5m;
    public int CurrentInterval { get; set; } = 1;
    public decimal EaseFactor { get; set; } = 2.5m;
    public int ReviewCount { get; set; } = 0;
    public string NextReviewDate { get; set; }
    public string LastReviewDate { get; set; }
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    //test
}
