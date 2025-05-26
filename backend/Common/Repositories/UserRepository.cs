using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Models;

namespace Common.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "User";

    public UserRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<User> GetByIdAsync(string userId)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["UserId"] = new AttributeValue { S = userId }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request);
        
        if (!response.IsItemSet)
            return null;

        return MapFromDynamoDb(response.Item);
    }

    public async Task<List<User>> GetAllAsync()
    {
        var request = new ScanRequest
        {
            TableName = _tableName
        };

        var response = await _dynamoDb.ScanAsync(request);
        return response.Items.Select(MapFromDynamoDb).ToList();
    }

    public async Task<User> CreateAsync(User user)
    {
        if (string.IsNullOrEmpty(user.UserId))
        {
            user.UserId = Guid.NewGuid().ToString();
        }
        
        user.CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

        var item = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = user.UserId },
            ["Email"] = new AttributeValue { S = user.Email },
            ["Username"] = new AttributeValue { S = user.Username },
            ["CreatedAt"] = new AttributeValue { S = user.CreatedAt.ToString("yyyy-MM-dd") },
            ["IsActive"] = new AttributeValue { BOOL = user.IsActive }
        };

        if (!string.IsNullOrEmpty(user.Image))
        {
            item["Image"] = new AttributeValue { S = user.Image };
        }

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };

        await _dynamoDb.PutItemAsync(request);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var updateExpression = "SET Email = :email, Username = :username, IsActive = :isActive";
        var expressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            [":email"] = new AttributeValue { S = user.Email },
            [":username"] = new AttributeValue { S = user.Username },
            [":isActive"] = new AttributeValue { BOOL = user.IsActive }
        };

        // Only include Image in the update if it's not empty
        if (!string.IsNullOrEmpty(user.Image))
        {
            updateExpression += ", Image = :image";
            expressionAttributeValues[":image"] = new AttributeValue { S = user.Image };
        }

        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["UserId"] = new AttributeValue { S = user.UserId }
            },
            UpdateExpression = updateExpression,
            ExpressionAttributeValues = expressionAttributeValues,
            ReturnValues = ReturnValue.ALL_NEW
        };

        var response = await _dynamoDb.UpdateItemAsync(request);
        return MapFromDynamoDb(response.Attributes);
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["UserId"] = new AttributeValue { S = userId }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    private User MapFromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        var user = new User
        {
            UserId = item.ContainsKey("UserId") ? item["UserId"].S : string.Empty,
            Email = item.ContainsKey("Email") ? item["Email"].S : string.Empty,
            Username = item.ContainsKey("Username") ? item["Username"].S : string.Empty,
            IsActive = item.ContainsKey("IsActive") && item["IsActive"].BOOL,
            Image = item.ContainsKey("Image") ? item["Image"].S : string.Empty
        };

        if (item.ContainsKey("CreatedAt"))
        {
            if (DateOnly.TryParse(item["CreatedAt"].S, out var date))
            {
                user.CreatedAt = date;
            }
        }

        return user;
    }
}
