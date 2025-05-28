using System;
using System.Collections.Generic;
using Common.Models;

namespace Common.Responses
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsVerified { get; set; }
        public string SubscriptionTier { get; set; }
        public DateTime SubscriptionExpiry { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    public class UserListResponse
    {
        public List<UserResponse> Users { get; set; } = new List<UserResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class UserStatsResponse
    {
        public int TotalUsers { get; set; }
        public int VerifiedUsers { get; set; }
        public int UnverifiedUsers { get; set; }
        public Dictionary<string, int> UsersBySubscriptionTier { get; set; } = new Dictionary<string, int>();
        public int UsersWithExpiredSubscriptions { get; set; }
        public int UsersCreatedToday { get; set; }
        public int UsersCreatedThisWeek { get; set; }
        public int UsersCreatedThisMonth { get; set; }
        public int ActiveUsersLastWeek { get; set; }
        public int ActiveUsersLastMonth { get; set; }
    }
} 