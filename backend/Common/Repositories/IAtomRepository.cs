using Common.Models;
using NeuroBrain.Common.Models;

namespace Common.Repositories;

public interface IAtomRepository
{
    Task<Atom> CreateAsync(Atom atom);
    Task<Atom> GetByIdAsync(string atomId, string userId);
    Task<List<Atom>> GetByUserIdAsync(string userId, bool includeArchived = false);
    Task<Atom> UpdateAsync(Atom atom);
    Task<bool> DeleteAsync(string atomId, string userId);
    Task<List<Atom>> SearchAsync(string userId, string query, bool includeContent = true, bool includeArchived = false);
    Task<List<Atom>> GetByTagAsync(string userId, string tag);
    Task<List<Atom>> GetPaginatedAsync(string userId, int page, int pageSize, string sortBy = "CreatedAt", string sortOrder = "desc", bool includeArchived = false);
    Task<int> GetCountAsync(string userId, bool includeArchived = false);
    Task<List<string>> GetAllTagsAsync(string userId);
}
