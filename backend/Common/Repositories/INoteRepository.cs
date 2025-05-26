using System.Collections.Generic;
using System.Threading.Tasks;
using NeuroBrain.Common.Models;

namespace NeuroBrain.Common.Repositories
{
    public interface INoteRepository
    {
        Task<Note> CreateAsync(Note note);
        Task<Note> GetByIdAsync(string noteId, string userId);
        Task<List<Note>> GetByUserIdAsync(string userId, bool includeArchived = false);
        Task<Note> UpdateAsync(Note note);
        Task<bool> DeleteAsync(string noteId, string userId);
        Task<List<Note>> SearchAsync(string userId, string query, bool includeContent = true, bool includeArchived = false);
        Task<List<Note>> GetByTagAsync(string userId, string tag);
        Task<List<Note>> GetPaginatedAsync(string userId, int page, int pageSize, string sortBy = "CreatedAt", string sortOrder = "desc", bool includeArchived = false);
        Task<bool> ArchiveAsync(string noteId, string userId, bool archive = true);
        Task<int> GetCountAsync(string userId, bool includeArchived = false);
        Task<List<string>> GetAllTagsAsync(string userId);
    }
}
