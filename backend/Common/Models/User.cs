using System;

namespace Common.Models;

public class User
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; } = false;
    public string SubscriptionTier { get; set; } = "free";
    public DateTime SubscriptionExpiry { get; set; } = DateTime.UtcNow.AddYears(100);
} 