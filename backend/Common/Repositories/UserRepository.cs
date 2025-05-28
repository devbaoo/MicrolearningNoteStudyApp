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

        var item = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = user.UserId },
            ["Email"] = new AttributeValue { S = user.Email },
            ["FirstName"] = new AttributeValue { S = user.FirstName },
            ["LastName"] = new AttributeValue { S = user.LastName },
            ["CreatedAt"] = new AttributeValue { S = user.CreatedAt.ToString("o") },
            ["LastLogin"] = new AttributeValue { S = user.LastLogin.ToString("o") },
            ["IsVerified"] = new AttributeValue { BOOL = user.IsVerified },
            ["SubscriptionTier"] = new AttributeValue { S = user.SubscriptionTier },
            ["SubscriptionExpiry"] = new AttributeValue { S = user.SubscriptionExpiry.ToString("o") }
        };

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
        var updateExpression = "SET Email = :email, FirstName = :firstName, LastName = :lastName, " +
                              "IsVerified = :isVerified, SubscriptionTier = :subscriptionTier, " +
                              "SubscriptionExpiry = :subscriptionExpiry, LastLogin = :lastLogin";
                              
        var expressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            [":email"] = new AttributeValue { S = user.Email },
            [":firstName"] = new AttributeValue { S = user.FirstName },
            [":lastName"] = new AttributeValue { S = user.LastName },
            [":isVerified"] = new AttributeValue { BOOL = user.IsVerified },
            [":subscriptionTier"] = new AttributeValue { S = user.SubscriptionTier },
            [":subscriptionExpiry"] = new AttributeValue { S = user.SubscriptionExpiry.ToString("o") },
            [":lastLogin"] = new AttributeValue { S = user.LastLogin.ToString("o") }
        };

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
            FirstName = item.ContainsKey("FirstName") ? item["FirstName"].S : string.Empty,
            LastName = item.ContainsKey("LastName") ? item["LastName"].S : string.Empty,
            IsVerified = item.ContainsKey("IsVerified") && item["IsVerified"].BOOL,
            SubscriptionTier = item.ContainsKey("SubscriptionTier") ? item["SubscriptionTier"].S : "free"
        };

        // Parse DateTime fields
        if (item.ContainsKey("CreatedAt") && DateTime.TryParse(item["CreatedAt"].S, out var createdAt))
        {
            user.CreatedAt = createdAt;
        }
        
        if (item.ContainsKey("LastLogin") && DateTime.TryParse(item["LastLogin"].S, out var lastLogin))
        {
            user.LastLogin = lastLogin;
        }
        
        if (item.ContainsKey("SubscriptionExpiry") && DateTime.TryParse(item["SubscriptionExpiry"].S, out var subscriptionExpiry))
        {
            user.SubscriptionExpiry = subscriptionExpiry;
        }

        return user;
    }
}
