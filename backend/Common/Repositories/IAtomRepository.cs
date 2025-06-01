using Common.Models;
using NeuroBrain.Common.Models;

namespace Common.Repositories;

public interface IAtomRepository
{
    Task<Atom> CreateAsync(Atom atom);
    Task<Atom?> GetByIdAsync(string atomId, string userId);
    Task<List<Atom>> GetByUserIdAsync(string userId);
    Task<Atom> UpdateAsync(Atom atom);
    Task<bool> DeleteAsync(string atomId, string userId);
    Task<List<Atom>> SearchAsync(string userId, string query, bool includeContent = true);
    Task<List<Atom>> GetByTagAsync(string userId, string tag);
    Task<List<Atom>> GetPaginatedAsync(string userId, int page, int pageSize, string sortBy = "CreatedAt", string sortOrder = "desc");
    Task<int> GetCountAsync(string userId);
    Task<List<string>> GetAllTagsAsync(string userId);
}
