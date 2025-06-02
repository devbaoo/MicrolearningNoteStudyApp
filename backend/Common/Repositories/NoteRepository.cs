using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using NeuroBrain.Common.Models;

namespace NeuroBrain.Common.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName = "Notes";

        public NoteRepository(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<Note> CreateAsync(Note note)
        {
            if (string.IsNullOrEmpty(note.NoteId))
            {
                note.NoteId = Guid.NewGuid().ToString();
            }

            var item = new Dictionary<string, AttributeValue>
            {
                ["NoteId"] = new AttributeValue { S = note.NoteId },
                ["UserId"] = new AttributeValue { S = note.UserId },
                ["Title"] = new AttributeValue { S = note.Title },
                ["Content"] = new AttributeValue { S = note.Content },
                ["Format"] = new AttributeValue { S = note.Format },
                ["Tags"] = new AttributeValue { SS = note.Tags.ToList() },
                ["CreatedAt"] = new AttributeValue { N = ((DateTimeOffset)note.CreatedAt).ToUnixTimeSeconds().ToString() },
                ["UpdatedAt"] = new AttributeValue { N = ((DateTimeOffset)note.UpdatedAt).ToUnixTimeSeconds().ToString() },
                ["IsArchived"] = new AttributeValue { BOOL = note.IsArchived },
                ["SourceType"] = new AttributeValue { S = note.SourceType ?? "manual" },
                ["QualityScore"] = new AttributeValue { N = note.QualityScore.ToString() },
                ["KnowledgeDensity"] = new AttributeValue { N = note.KnowledgeDensity.ToString() },
                ["WordCount"] = new AttributeValue { N = note.WordCount.ToString() },
                ["AtomCount"] = new AttributeValue { N = note.AtomCount.ToString() }
            };

            if (!string.IsNullOrEmpty(note.SourceUrl))
            {
                item["SourceUrl"] = new AttributeValue { S = note.SourceUrl };
            }

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            };

            await _dynamoDb.PutItemAsync(request);
            return note;
        }

        public async Task<Note> GetByIdAsync(string noteId, string userId)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["NoteId"] = new AttributeValue { S = noteId },
                    ["UserId"] = new AttributeValue { S = userId }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);
            
            if (!response.IsItemSet)
                return null;

            return MapFromDynamoDb(response.Item);
        }

        public async Task<List<Note>> GetByUserIdAsync(string userId, bool includeArchived = false)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = "UserId-CreatedAt-index",
                KeyConditionExpression = "UserId = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":userId"] = new AttributeValue { S = userId }
                },
                ScanIndexForward = false // Sort by CreatedAt descending
            };

            if (!includeArchived)
            {
                request.FilterExpression = "IsArchived = :archived";
                request.ExpressionAttributeValues[":archived"] = new AttributeValue { BOOL = false };
            }

            var response = await _dynamoDb.QueryAsync(request);
            return response.Items.Select(MapFromDynamoDb).ToList();
        }

        public async Task<Note> UpdateAsync(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;

            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["NoteId"] = new AttributeValue { S = note.NoteId },
                    ["UserId"] = new AttributeValue { S = note.UserId }
                },
                UpdateExpression = "SET Title = :title, Content = :content, Format = :format, Tags = :tags, UpdatedAt = :updatedAt, QualityScore = :qualityScore, KnowledgeDensity = :knowledgeDensity, WordCount = :wordCount, AtomCount = :atomCount",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":title"] = new AttributeValue { S = note.Title },
                    [":content"] = new AttributeValue { S = note.Content },
                    [":format"] = new AttributeValue { S = note.Format },
                    [":tags"] = new AttributeValue { SS = note.Tags.ToList() },
                    [":updatedAt"] = new AttributeValue { N = ((DateTimeOffset)note.UpdatedAt).ToUnixTimeSeconds().ToString() },
                    [":qualityScore"] = new AttributeValue { N = note.QualityScore.ToString() },
                    [":knowledgeDensity"] = new AttributeValue { N = note.KnowledgeDensity.ToString() },
                    [":wordCount"] = new AttributeValue { N = note.WordCount.ToString() },
                    [":atomCount"] = new AttributeValue { N = note.AtomCount.ToString() }
                },
                ReturnValues = ReturnValue.ALL_NEW
            };

            var response = await _dynamoDb.UpdateItemAsync(request);
            return MapFromDynamoDb(response.Attributes);
        }

        public async Task<bool> DeleteAsync(string noteId, string userId)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["NoteId"] = new AttributeValue { S = noteId },
                    ["UserId"] = new AttributeValue { S = userId }
                }
            };

            var response = await _dynamoDb.DeleteItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<List<Note>> SearchAsync(string userId, string query, bool includeContent = true, bool includeArchived = false)
        {
            var notes = await GetByUserIdAsync(userId, includeArchived);
            
            var searchResults = notes.Where(note =>
                note.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (includeContent && note.Content.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                note.Tags.Any(tag => tag.Contains(query, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            return searchResults;
        }

        public async Task<List<Note>> GetByTagAsync(string userId, string tag)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = "UserId-CreatedAt-index",
                KeyConditionExpression = "UserId = :userId",
                FilterExpression = "contains(Tags, :tag)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":userId"] = new AttributeValue { S = userId },
                    [":tag"] = new AttributeValue { S = tag }
                }
            };

            var response = await _dynamoDb.QueryAsync(request);
            return response.Items.Select(MapFromDynamoDb).ToList();
        }

        public async Task<List<Note>> GetPaginatedAsync(string userId, int page, int pageSize, string sortBy = "CreatedAt", string sortOrder = "desc", bool includeArchived = false)
        {
            var allNotes = await GetByUserIdAsync(userId, includeArchived);
            
            // Simple in-memory pagination - in production, you'd use DynamoDB pagination
            var sortedNotes = sortOrder.ToLower() == "asc" 
                ? allNotes.OrderBy(n => n.CreatedAt).ToList()
                : allNotes.OrderByDescending(n => n.CreatedAt).ToList();

            return sortedNotes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<bool> ArchiveAsync(string noteId, string userId, bool archive = true)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["NoteId"] = new AttributeValue { S = noteId },
                    ["UserId"] = new AttributeValue { S = userId }
                },
                UpdateExpression = "SET IsArchived = :archived, UpdatedAt = :updatedAt",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":archived"] = new AttributeValue { BOOL = archive },
                    [":updatedAt"] = new AttributeValue { N = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
                }
            };

            var response = await _dynamoDb.UpdateItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<int> GetCountAsync(string userId, bool includeArchived = false)
        {
            var notes = await GetByUserIdAsync(userId, includeArchived);
            return notes.Count;
        }

        public async Task<List<string>> GetAllTagsAsync(string userId)
        {
            var notes = await GetByUserIdAsync(userId, true);
            var allTags = new HashSet<string>();
            
            foreach (var note in notes)
            {
                foreach (var tag in note.Tags)
                {
                    allTags.Add(tag);
                }
            }

            return allTags.ToList();
        }

        private Note MapFromDynamoDb(Dictionary<string, AttributeValue> item)
        {
            return new Note
            {
                NoteId = item["NoteId"].S,
                UserId = item["UserId"].S,
                Title = item["Title"].S,
                Content = item["Content"].S,
                Format = item["Format"].S,
                Tags = item["Tags"].SS.ToHashSet(),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(item["CreatedAt"].N)).DateTime,
                UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(item["UpdatedAt"].N)).DateTime,
                IsArchived = item["IsArchived"].BOOL,
                SourceType = item.ContainsKey("SourceType") ? item["SourceType"].S : "manual",
                SourceUrl = item.ContainsKey("SourceUrl") ? item["SourceUrl"].S : null,
                QualityScore = double.Parse(item["QualityScore"].N),
                KnowledgeDensity = double.Parse(item["KnowledgeDensity"].N),
                WordCount = int.Parse(item["WordCount"].N),
                AtomCount = int.Parse(item["AtomCount"].N)
            };
        }
    }
}
