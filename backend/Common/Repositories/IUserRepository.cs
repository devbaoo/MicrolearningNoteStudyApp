using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(string userId);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(string userId);
}
