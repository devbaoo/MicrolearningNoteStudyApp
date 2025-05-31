using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Models;
using NeuroBrain.Common.Models;

namespace Common.Repositories;

public class AtomRepository : IAtomRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "Atoms";

    public AtomRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<Atom> CreateAsync(Atom atom)
    {
        if (string.IsNullOrEmpty(atom.AtomId))
        {
            atom.AtomId = Guid.NewGuid().ToString();
        }

        var item = new Dictionary<string, AttributeValue>
        {

        };

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };

        await _dynamoDb.PutItemAsync(request);
        return atom;
    }

    public async Task<Atom> GetByIdAsync(string atomId, string userId)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["AtomId"] = new AttributeValue { S = atomId },
                ["UserId"] = new AttributeValue { S = userId }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request);

        if (!response.IsItemSet)
            return null;

        return MapFromDynamoDb(response.Item);
    }

    public async Task<List<Atom>> GetByUserIdAsync(string userId)
    {
        var request = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "userId-createdAt-index",
            KeyConditionExpression = "UserId = :userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":userId"] = new AttributeValue { S = userId }
            },
            ScanIndexForward = false // Sort by CreatedAt descending
        };

        var response = await _dynamoDb.QueryAsync(request);
        return response.Items.Select(MapFromDynamoDb).ToList();
    }

    public async Task<Atom> UpdateAsync(Atom atom)
    {
        atom.UpdatedAt = DateTime.UtcNow;

        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["AtomId"] = new AttributeValue { S = atom.AtomId },
                ["UserId"] = new AttributeValue { S = atom.UserId }
            },
            UpdateExpression = "",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {

            },
            ReturnValues = ReturnValue.ALL_NEW
        };

        var response = await _dynamoDb.UpdateItemAsync(request);
        return MapFromDynamoDb(response.Attributes);
    }

    public async Task<bool> DeleteAsync(string atomId, string userId)
    {
        var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["AtomId"] = new AttributeValue { S = atomId },
                ["UserId"] = new AttributeValue { S = userId }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<List<Atom>> SearchAsync(string userId, string query, bool includeContent = true)
    {
        var atoms = await GetByUserIdAsync(userId);

        var searchResults = atoms.Where(atom =>
            (includeContent && atom.Content.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            atom.Tags.Any(tag => tag.Contains(query, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        return searchResults;
    }

    public async Task<List<Atom>> GetByTagAsync(string userId, string tag)
    {
        var request = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "userId-createdAt-index",
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

    public async Task<List<Atom>> GetPaginatedAsync(string userId, int page, int pageSize, string sortBy = "CreatedAt", string sortOrder = "desc")
    {
        var allAtoms = await GetByUserIdAsync(userId);

        // Simple in-memory pagination - in production, you'd use DynamoDB pagination
        var sortedAtoms = sortOrder.ToLower() == "asc"
            ? allAtoms.OrderBy(n => n.CreatedAt).ToList()
            : allAtoms.OrderByDescending(n => n.CreatedAt).ToList();

        return sortedAtoms
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public async Task<bool> ArchiveAsync(string atomId, string userId, bool archive = true)
    {
        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["AtomId"] = new AttributeValue { S = atomId },
                ["UserId"] = new AttributeValue { S = userId }
            },
            UpdateExpression = "SET UpdatedAt = :updatedAt",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":updatedAt"] = new AttributeValue { N = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
            }
        };

        var response = await _dynamoDb.UpdateItemAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<int> GetCountAsync(string userId)
    {
        var atoms = await GetByUserIdAsync(userId);
        return atoms.Count;
    }

    public async Task<List<string>> GetAllTagsAsync(string userId)
    {
        var notes = await GetByUserIdAsync(userId);
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

    private Atom MapFromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        return new Atom
        {
            AtomId = item["AtomId"].S,
            UserId = item["UserId"].S,
            Content = item["Content"].S,
            Type = item["Type"].S,
            Tags = item["Tags"].SS.ToHashSet(),
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(item["CreatedAt"].N)).DateTime,
            UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(item["UpdatedAt"].N)).DateTime,
            NextReviewDate = item["NextReviewDate"].S,
            LastReviewDate = item["LastReviewDate"].S,
            ImportanceScore = decimal.Parse(item["ImportanceScore"].N),
            DifficultyScore = decimal.Parse(item["DifficultyScore"].N),
            CurrentInterval = int.Parse(item["CurrentInterval"].N),
            EaseFactor = decimal.Parse(item["EaseFactor"].N),
            ReviewCount = int.Parse(item["ReviewCount"].N)
        };
    }
}
