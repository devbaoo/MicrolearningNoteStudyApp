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
    public List<string> Tags { get; set; } = new List<string>();
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    //test
}
