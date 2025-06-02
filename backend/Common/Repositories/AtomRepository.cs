using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Models;

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
            ["AtomId"] = new AttributeValue { S = atom.AtomId },
            ["UserId"] = new AttributeValue { S = atom.UserId },
            ["Content"] = new AttributeValue { S = atom.Content },
            ["Type"] = new AttributeValue { S = atom.Type ?? "concept" },
            ["Tags"] = new AttributeValue { SS = atom.Tags?.ToList() ?? new List<string>() },
            ["CreatedAt"] = new AttributeValue { N = ((DateTimeOffset)atom.CreatedAt).ToUnixTimeSeconds().ToString() },
            ["UpdatedAt"] = new AttributeValue { N = ((DateTimeOffset)(atom.UpdatedAt ?? DateTime.UtcNow)).ToUnixTimeSeconds().ToString() },
            ["NextReviewDate"] = new AttributeValue { S = atom.NextReviewDate ?? "" },
            ["LastReviewDate"] = new AttributeValue { S = atom.LastReviewDate ?? "" },
            ["ImportanceScore"] = new AttributeValue { N = (atom.ImportanceScore ?? 0.5m).ToString() },
            ["DifficultyScore"] = new AttributeValue { N = (atom.DifficultyScore ?? 0.5m).ToString() },
            ["EaseFactor"] = new AttributeValue { N = atom.EaseFactor.ToString() },
            ["CurrentInterval"] = new AttributeValue { N = atom.CurrentInterval.ToString() },
            ["ReviewCount"] = new AttributeValue { N = atom.ReviewCount.ToString() }
        };

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };

        await _dynamoDb.PutItemAsync(request);
        return atom;
    }

    public async Task<Atom?> GetByIdAsync(string atomId, string userId)
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
            // IndexName = "userId-createdAt-index",
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
            UpdateExpression = "SET Content = :content, Type = :type, Tags = :tags, UpdatedAt = :updatedAt, NextReviewDate = :nextReviewDate, LastReviewDate = :lastReviewDate, ImportanceScore = :importanceScore, DifficultyScore = :difficultyScore, EaseFactor = :easeFactor, CurrentInterval = :currentInterval, ReviewCount = :reviewCount",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":content"] = new AttributeValue { S = atom.Content },
                [":type"] = new AttributeValue { S = atom.Type ?? "concept" },
                [":tags"] = new AttributeValue { SS = atom.Tags?.ToList() ?? new List<string>() },
                [":updatedAt"] = new AttributeValue { N = ((DateTimeOffset)(atom.UpdatedAt ?? DateTime.UtcNow)).ToUnixTimeSeconds().ToString() },
                [":nextReviewDate"] = new AttributeValue { S = atom.NextReviewDate ?? "" },
                [":lastReviewDate"] = new AttributeValue { S = atom.LastReviewDate ?? "" },
                [":importanceScore"] = new AttributeValue { N = (atom.ImportanceScore ?? 0.5m).ToString() },
                [":difficultyScore"] = new AttributeValue { N = (atom.DifficultyScore ?? 0.5m).ToString() },
                [":easeFactor"] = new AttributeValue { N = atom.EaseFactor.ToString() },
                [":currentInterval"] = new AttributeValue { N = atom.CurrentInterval.ToString() },
                [":reviewCount"] = new AttributeValue { N = atom.ReviewCount.ToString() }
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
            // IndexName = "userId-createdAt-index",
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

    public async Task<int> GetCountAsync(string userId)
    {
        var atoms = await GetByUserIdAsync(userId);
        return atoms.Count;
    }

    public async Task<List<string>> GetAllTagsAsync(string userId)
    {
        var atoms = await GetByUserIdAsync(userId);
        var allTags = new HashSet<string>();

        foreach (var atom in atoms)
        {
            foreach (var tag in atom.Tags)
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
            Tags = item.ContainsKey("Tags") ? item["Tags"].SS.ToHashSet() : new HashSet<string>(),
            CreatedAt = TryGetUnixDateTime(item, "CreatedAt") ?? DateTime.MinValue,
            UpdatedAt = TryGetUnixDateTime(item, "UpdatedAt") ?? DateTime.MinValue,
            NextReviewDate = item["NextReviewDate"].S,
            LastReviewDate = item["LastReviewDate"].S,
            ImportanceScore = item.ContainsKey("ImportanceScore") ? decimal.Parse(item["ImportanceScore"].N) : null,
            DifficultyScore = item.ContainsKey("DifficultyScore") ? decimal.Parse(item["DifficultyScore"].N) : null,
            EaseFactor = item.ContainsKey("EaseFactor") ? decimal.Parse(item["EaseFactor"].N) : 2.5m,
            CurrentInterval = item.ContainsKey("CurrentInterval") ? int.Parse(item["CurrentInterval"].N) : 1,
            ReviewCount = item.ContainsKey("ReviewCount") ? int.Parse(item["ReviewCount"].N) : 0
        };
    }

    private DateTime? TryGetUnixDateTime(Dictionary<string, AttributeValue> item, string key)
    {
        if (item.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value.N) && long.TryParse(value.N, out var unixTime))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        }

        return null;
    }
}
