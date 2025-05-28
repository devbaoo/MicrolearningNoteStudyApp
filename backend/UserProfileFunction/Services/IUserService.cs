using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserProfileFunction.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user, string userId);
        Task<bool> DeleteAsync(string userId);
    }
} 