using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Requests
{
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }
        
        public bool IsVerified { get; set; } = false;
        
        public string SubscriptionTier { get; set; } = "free";
        
        public DateTime? SubscriptionExpiry { get; set; }
    }

    public class UpdateUserRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }
        
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }
        
        public bool? IsVerified { get; set; }
        
        public string SubscriptionTier { get; set; }
        
        public DateTime? SubscriptionExpiry { get; set; }
        
        public DateTime? LastLogin { get; set; }
    }

    public class GetUsersRequest
    {
        public bool IncludeUnverified { get; set; } = true;
        public string SubscriptionTier { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }
} 