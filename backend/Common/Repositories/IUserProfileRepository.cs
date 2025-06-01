using System.Threading.Tasks;
using Common.Models;

namespace Common.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile> GetByUserIdAsync(string userId);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<UserProfile> UpdateAsync(UserProfile userProfile);
    Task<bool> DeleteAsync(string userId);
} 