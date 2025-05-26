using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuroBrain.Common.Models;
using NeuroBrain.Common.Repositories;
using NeuroBrain.Common.Requests;
using NeuroBrain.Common.Responses;

namespace NeuroBrain.NoteManagementFunction.Services
{
    public class NoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteResponse> CreateNoteAsync(CreateNoteRequest request, string userId)
        {
            var note = new Note
            {
                UserId = userId,
                Title = request.Title,
                Content = request.Content,
                Format = request.Format,
                Tags = request.Tags?.ToHashSet() ?? new HashSet<string>(),
                SourceType = request.SourceType,
                SourceUrl = request.SourceUrl,
                WordCount = CountWords(request.Content),
                QualityScore = CalculateQualityScore(request.Content),
                KnowledgeDensity = CalculateKnowledgeDensity(request.Content)
            };

            var createdNote = await _noteRepository.CreateAsync(note);
            return MapToResponse(createdNote);
        }

        public async Task<NoteResponse> GetNoteByIdAsync(string noteId, string userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId, userId);
            return note != null ? MapToResponse(note) : null;
        }

        public async Task<NoteListResponse> GetNotesAsync(GetNotesRequest request, string userId)
        {
            var notes = await _noteRepository.GetPaginatedAsync(
                userId, 
                request.Page, 
                request.PageSize, 
                request.SortBy, 
                request.SortOrder, 
                request.IncludeArchived
            );

            var totalCount = await _noteRepository.GetCountAsync(userId, request.IncludeArchived);

            return new NoteListResponse
            {
                Notes = notes.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        public async Task<NoteResponse> UpdateNoteAsync(UpdateNoteRequest request, string userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(request.NoteId, userId);
            if (existingNote == null)
                return null;

            existingNote.Title = request.Title ?? existingNote.Title;
            existingNote.Content = request.Content ?? existingNote.Content;
            existingNote.Format = request.Format ?? existingNote.Format;
            existingNote.Tags = request.Tags?.ToHashSet() ?? existingNote.Tags;
            existingNote.WordCount = CountWords(existingNote.Content);
            existingNote.QualityScore = CalculateQualityScore(existingNote.Content);
            existingNote.KnowledgeDensity = CalculateKnowledgeDensity(existingNote.Content);

            var updatedNote = await _noteRepository.UpdateAsync(existingNote);
            return MapToResponse(updatedNote);
        }

        public async Task<bool> DeleteNoteAsync(string noteId, string userId)
        {
            return await _noteRepository.DeleteAsync(noteId, userId);
        }

        public async Task<NoteListResponse> SearchNotesAsync(SearchNotesRequest request, string userId)
        {
            var notes = await _noteRepository.SearchAsync(
                userId, 
                request.Query, 
                request.IncludeContent, 
                request.IncludeArchived
            );

            // Apply tag filter if specified
            if (request.Tags?.Any() == true)
            {
                notes = notes.Where(n => request.Tags.Any(tag => n.Tags.Contains(tag))).ToList();
            }

            // Apply pagination
            var paginatedNotes = notes
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new NoteListResponse
            {
                Notes = paginatedNotes.Select(MapToResponse).ToList(),
                TotalCount = notes.Count,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)notes.Count / request.PageSize)
            };
        }

        public async Task<bool> ArchiveNoteAsync(string noteId, string userId, bool archive = true)
        {
            return await _noteRepository.ArchiveAsync(noteId, userId, archive);
        }

        public async Task<List<string>> GetAllTagsAsync(string userId)
        {
            return await _noteRepository.GetAllTagsAsync(userId);
        }

        public async Task<NoteStatsResponse> GetStatsAsync(string userId)
        {
            var allNotes = await _noteRepository.GetByUserIdAsync(userId, true);
            var activenotes = allNotes.Where(n => !n.IsArchived).ToList();

            return new NoteStatsResponse
            {
                TotalNotes = activenotes.Count,
                ArchivedNotes = allNotes.Count(n => n.IsArchived),
                TotalWords = activenotes.Sum(n => n.WordCount),
                TotalAtoms = activenotes.Sum(n => n.AtomCount),
                AverageQualityScore = activenotes.Any() ? activenotes.Average(n => n.QualityScore) : 0,
                TopTags = await GetTopTags(userId, 10)
            };
        }

        private async Task<List<string>> GetTopTags(string userId, int count)
        {
            var allTags = await _noteRepository.GetAllTagsAsync(userId);
            // In a real implementation, you'd count tag usage and return the most used ones
            return allTags.Take(count).ToList();
        }

        private NoteResponse MapToResponse(Note note)
        {
            return new NoteResponse
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Content = note.Content,
                Format = note.Format,
                Tags = note.Tags.ToList(),
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                IsArchived = note.IsArchived,
                SourceType = note.SourceType,
                SourceUrl = note.SourceUrl,
                QualityScore = note.QualityScore,
                KnowledgeDensity = note.KnowledgeDensity,
                WordCount = note.WordCount,
                AtomCount = note.AtomCount
            };
        }

        private int CountWords(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 0;

            return content.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private double CalculateQualityScore(string content)
        {
            // Simple quality calculation - in production, use NLP analysis
            if (string.IsNullOrWhiteSpace(content))
                return 0.0;

            var wordCount = CountWords(content);
            var sentenceCount = content.Split('.', '!', '?').Length;
            
            // Basic quality metrics
            var avgWordsPerSentence = sentenceCount > 0 ? (double)wordCount / sentenceCount : 0;
            var hasStructure = content.Contains('\n') || content.Contains('#');
            
            var score = Math.Min(1.0, (avgWordsPerSentence / 20.0) + (hasStructure ? 0.2 : 0));
            return Math.Round(score, 2);
        }

        private double CalculateKnowledgeDensity(string content)
        {
            // Simple density calculation - in production, use NLP to identify concepts
            if (string.IsNullOrWhiteSpace(content))
                return 0.0;

            var wordCount = CountWords(content);
            // Assume every 10 words contains 1 meaningful concept
            var estimatedConcepts = wordCount / 10.0;
            var density = Math.Min(1.0, estimatedConcepts / wordCount);
            
            return Math.Round(density, 2);
        }
    }
} 