using System;

namespace Common.Models;

public class UserProfile
{
    public string UserId { get; set; } = string.Empty; // Foreign key to User table
    public string Avatar { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string Bio { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 