using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Common.Repositories;
using Common.Responses;
using NeuroBrain.Common.Models;
using NeuroBrain.Common.Repositories;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;
using static Common.Requests.AtomRequests;
using static Common.Responses.AtomResponses;

namespace NeuroBrain.AtomManagementFunction.Services
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
                SourceNoteId = request.SourceNoteId,
                EmbeddingVector = request.EmbeddingVector ?? Array.Empty<byte>(),
                IsMannuallyCreated = request.IsMannuallyCreated,
                CreatedAt = DateTime.Now,
                MasteryLevel = request.MasteryLevel,
                AccessCount = request.AccessCount
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
                request.SortOrder,
                request.IncludeArchived
            );

            var totalCount = await _atomRepository.GetCountAsync(userId, request.IncludeArchived);

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
            existingAtom.SourceNoteId = request.SourceNoteId ?? existingAtom.SourceNoteId;
            existingAtom.ImportanceScore = request.ImportanceScore ?? existingAtom.ImportanceScore;
            existingAtom.DifficultyScore = request.DifficultyScore ?? existingAtom.DifficultyScore;
            existingAtom.MasteryLevel = request.MasteryLevel ?? existingAtom.MasteryLevel;
            existingAtom.EmbeddingVector = request.EmbeddingVector ?? existingAtom.EmbeddingVector;
            existingAtom.UpdatedAt = DateTime.Now;

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
                SourceNoteId = atom.SourceNoteId,
                ImportanceScore = atom.ImportanceScore,
                DifficultyScore = atom.DifficultyScore,
                MasteryLevel = atom.MasteryLevel,
                EmbeddingVector = atom.EmbeddingVector,
                AccessCount = atom.AccessCount,
                Tags = atom.Tags.ToList(),
                CreatedAt = atom.CreatedAt,
                UpdatedAt = atom.UpdatedAt,
                LastAccessed = atom.LastAccessed,
                IsMannuallyCreated = atom.IsMannuallyCreated
            };
        }
    }
}