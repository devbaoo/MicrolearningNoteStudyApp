using Common.Models;
using Common.Repositories;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace AtomManagementFunction.Services
{
    public class AtomService
    {
        private readonly IAtomRepository _atomRepository;

        public AtomService(IAtomRepository atomRepository)
        {
            _atomRepository = atomRepository;
        }

        public async Task<AtomResponse> CreateAtomAsync(CreateAtomRequest request, string userId)
        {
            var atom = new Atom
            {
                UserId = userId,
                Content = request.Content,
                Type = request.Type,
                ImportanceScore = request.ImportanceScore,
                DifficultyScore = request.DifficultyScore,
                Tags = request.Tags?.ToHashSet() ?? new HashSet<string>(),
                CurrentInterval = request.CurrentInterval,
                EaseFactor = request.EaseFactor,
                ReviewCount = request.ReviewCount,
                NextReviewDate = request.NextReviewDate,
                LastReviewDate = request.LastReviewDate
            };

            var createdAtom = await _atomRepository.CreateAsync(atom);
            return MapToResponse(createdAtom);
        }

        public async Task<AtomResponse?> GetAtomByIdAsync(string atomId, string userId)
        {
            var atom = await _atomRepository.GetByIdAsync(atomId, userId);
            return atom != null ? MapToResponse(atom) : null;
        }

        public async Task<AtomListResponse> GetAtomsAsync(GetAtomsRequest request, string userId)
        {
            var atoms = await _atomRepository.GetPaginatedAsync(
                userId,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder
            );

            var totalCount = await _atomRepository.GetCountAsync(userId);

            return new AtomListResponse
            {
                Atoms = atoms.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        public async Task<AtomResponse?> UpdateAtomAsync(UpdateAtomRequest request, string userId)
        {
            var existingAtom = await _atomRepository.GetByIdAsync(request.AtomId, userId);
            if (existingAtom == null)
                return null;

            existingAtom.Content = request.Content ?? existingAtom.Content;
            existingAtom.Type = request.Type ?? existingAtom.Type;
            existingAtom.Tags = request.Tags?.ToHashSet() ?? existingAtom.Tags;
            existingAtom.ImportanceScore = request.ImportanceScore ?? existingAtom.ImportanceScore;
            existingAtom.DifficultyScore = request.DifficultyScore ?? existingAtom.DifficultyScore;
            existingAtom.UpdatedAt = DateTime.Now;
            existingAtom.CurrentInterval = request.CurrentInterval;
            existingAtom.EaseFactor = request.EaseFactor;
            /*       existingAtom.ReviewCount = request.ReviewCount;*/
            existingAtom.NextReviewDate = request.NextReviewDate ?? existingAtom.NextReviewDate;
            existingAtom.LastReviewDate = request.LastReviewDate ?? existingAtom.LastReviewDate;

            var updatedAtom = await _atomRepository.UpdateAsync(existingAtom);
            return MapToResponse(updatedAtom);
        }

        public async Task<bool> DeleteAtomAsync(string atomId, string userId)
        {
            return await _atomRepository.DeleteAsync(atomId, userId);
        }

        public async Task<AtomListResponse> SearchAtomsAsync(SearchAtomsRequest request, string userId)
        {
            var atoms = await _atomRepository.SearchAsync(
                userId,
                request.Query,
                request.IncludeContent
            );

            // Apply tag filter if specified
            if (request.Tags?.Any() == true)
            {
                atoms = atoms.Where(n => request.Tags.Any(tag => n.Tags.Contains(tag))).ToList();
            }

            // Apply pagination
            var paginatedAtoms = atoms
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new AtomListResponse
            {
                Atoms = paginatedAtoms.Select(MapToResponse).ToList(),
                TotalCount = atoms.Count,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)atoms.Count / request.PageSize)
            };
        }

        public async Task<List<string>> GetAllTagsAsync(string userId)
        {
            return await _atomRepository.GetAllTagsAsync(userId);
        }

        private async Task<List<string>> GetTopTags(string userId, int count)
        {
            var allTags = await _atomRepository.GetAllTagsAsync(userId);
            // In a real implementation, you'd count tag usage and return the most used ones
            return allTags.Take(count).ToList();
        }

        private AtomResponse MapToResponse(Atom atom)
        {
            return new AtomResponse
            {
                AtomId = atom.AtomId,
                UserId = atom.UserId,
                Content = atom.Content,
                Type = atom.Type,
                ImportanceScore = atom.ImportanceScore,
                DifficultyScore = atom.DifficultyScore,
                CurrentInterval = atom.CurrentInterval,
                EaseFactor = atom.EaseFactor,
                ReviewCount = atom.ReviewCount,
                NextReviewDate = atom.NextReviewDate,
                LastReviewDate = atom.LastReviewDate,
                Tags = atom.Tags.ToList(),
                CreatedAt = atom.CreatedAt,
                UpdatedAt = atom.UpdatedAt,
            };
        }
    }
}