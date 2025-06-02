using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Models;

namespace Common.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "UserProfile";

    public UserProfileRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<UserProfile> GetByUserIdAsync(string userId)
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

    public async Task<UserProfile> CreateAsync(UserProfile userProfile)
    {
        userProfile.UpdatedAt = DateTime.UtcNow;
        
        var item = new Dictionary<string, AttributeValue>
        {
            ["UserId"] = new AttributeValue { S = userProfile.UserId },
            ["Avatar"] = new AttributeValue { S = userProfile.Avatar ?? string.Empty },
            ["Timezone"] = new AttributeValue { S = userProfile.Timezone ?? "UTC" },
            ["Language"] = new AttributeValue { S = userProfile.Language ?? "en" },
            ["Bio"] = new AttributeValue { S = userProfile.Bio ?? string.Empty },
            ["UpdatedAt"] = new AttributeValue { S = userProfile.UpdatedAt.ToString("o") }
        };

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };

        await _dynamoDb.PutItemAsync(request);
        return userProfile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
    {
        userProfile.UpdatedAt = DateTime.UtcNow;
        
        var updateExpression = "SET Avatar = :avatar, Timezone = :timezone, " + 
                               "Language = :language, Bio = :bio, UpdatedAt = :updatedAt";
                              
        var expressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            [":avatar"] = new AttributeValue { S = userProfile.Avatar ?? string.Empty },
            [":timezone"] = new AttributeValue { S = userProfile.Timezone ?? "UTC" },
            [":language"] = new AttributeValue { S = userProfile.Language ?? "en" },
            [":bio"] = new AttributeValue { S = userProfile.Bio ?? string.Empty },
            [":updatedAt"] = new AttributeValue { S = userProfile.UpdatedAt.ToString("o") }
        };

        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["UserId"] = new AttributeValue { S = userProfile.UserId }
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

    private UserProfile MapFromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        var userProfile = new UserProfile
        {
            UserId = item.ContainsKey("UserId") ? item["UserId"].S : string.Empty,
            Avatar = item.ContainsKey("Avatar") ? item["Avatar"].S : string.Empty,
            Timezone = item.ContainsKey("Timezone") ? item["Timezone"].S : "UTC",
            Language = item.ContainsKey("Language") ? item["Language"].S : "en",
            Bio = item.ContainsKey("Bio") ? item["Bio"].S : string.Empty
        };

        // Parse DateTime fields
        if (item.ContainsKey("UpdatedAt") && DateTime.TryParse(item["UpdatedAt"].S, out var updatedAt))
        {
            userProfile.UpdatedAt = updatedAt;
        }

        return userProfile;
    }
} 