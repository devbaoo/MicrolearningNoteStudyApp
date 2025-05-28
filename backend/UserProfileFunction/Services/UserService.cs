using Common.Models;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserProfileFunction.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            // Set default values if not provided
            if (user.CreatedAt == default)
            {
                user.CreatedAt = DateTime.UtcNow;
            }
            
            if (user.LastLogin == default)
            {
                user.LastLogin = DateTime.UtcNow;
            }
            
            if (string.IsNullOrEmpty(user.SubscriptionTier))
            {
                user.SubscriptionTier = "free";
            }
            
            if (user.SubscriptionExpiry == default)
            {
                // Default to a far future date for free tier
                user.SubscriptionExpiry = DateTime.UtcNow.AddYears(100);
            }

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User> UpdateAsync(User updatedUser, string userId)
        {
            // Get existing user to maintain creation date and other immutable fields
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return null;
            }

            // Ensure the userId matches
            updatedUser.UserId = userId;
            
            // Preserve immutable fields
            updatedUser.CreatedAt = existingUser.CreatedAt;
            
            return await _userRepository.UpdateAsync(updatedUser);
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }
    }
} 