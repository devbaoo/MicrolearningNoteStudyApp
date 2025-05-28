using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Moq;
using Common.Repositories;
using Common.Models;
using System.Net;
using System.Collections.Generic;

namespace UserProfileFunction.Tests;

public class FunctionTests
{
    [Fact]
    public async Task TestDefaultRoute_ReturnsNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new()
            {
                Http = new()
                {
                    Method = "GET"
                }
            },
            // No path parameters - should trigger the default case in the switch
        };
        var context = new TestLambdaContext();
        var function = new Function(mockRepo.Object);

        // Act
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Route not found", response.Body);
    }
    
    [Fact]
    public async Task TestUpdateUser_Success()
    {
        // Arrange
        var userId = "user123";
        var existingUser = new User
        {
            UserId = userId,
            Email = "old@example.com",
            Username = "oldusername",
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            IsActive = true,
            Image = "oldimage.jpg"
        };
        
        var updatedUserData = new User
        {
            UserId = userId,
            Email = "new@example.com",
            Username = "newusername",
            IsActive = true,
            Image = "newimage.jpg"
        };
        
        var userAfterUpdate = new User
        {
            UserId = userId,
            Email = "new@example.com",
            Username = "newusername",
            CreatedAt = existingUser.CreatedAt, // Should preserve the original creation date
            IsActive = true,
            Image = "newimage.jpg"
        };
        
        // Setup mock repository
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(userAfterUpdate);
        
        // Create request with path parameters and body
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new()
            {
                Http = new()
                {
                    Method = "PUT"
                }
            },
            PathParameters = new Dictionary<string, string>
            {
                { "userId", userId }
            },
            Body = JsonSerializer.Serialize(updatedUserData, new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            })
        };
        
        var context = new TestLambdaContext();
        var function = new Function(mockRepo.Object);
        
        // Act
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        
        // Deserialize and verify the response
        var options = new JsonSerializerOptions
        {
            Converters = { new DateOnlyJsonConverter() }
        };
        var returnedUser = JsonSerializer.Deserialize<User>(response.Body, options);
        
        Assert.NotNull(returnedUser);
        Assert.Equal(userId, returnedUser.UserId);
        Assert.Equal("new@example.com", returnedUser.Email);
        Assert.Equal("newusername", returnedUser.Username);
        Assert.Equal(existingUser.CreatedAt, returnedUser.CreatedAt); // Verify creation date was preserved
        Assert.Equal("newimage.jpg", returnedUser.Image);
        
        // Verify the repository was called with correct parameters
        mockRepo.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        mockRepo.Verify(repo => repo.UpdateAsync(It.Is<User>(u => 
            u.UserId == userId && 
            u.Email == "new@example.com" && 
            u.Username == "newusername" &&
            u.CreatedAt == existingUser.CreatedAt && 
            u.Image == "newimage.jpg")), 
            Times.Once);
    }
    
    [Fact]
    public async Task TestUpdateUser_UserNotFound()
    {
        // Arrange
        var userId = "nonexistentUser";
        
        var updatedUserData = new User
        {
            UserId = userId,
            Email = "new@example.com",
            Username = "newusername",
            IsActive = true
        };
        
        // Setup mock repository to return null (user not found)
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User)null);
        
        // Create request
        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new()
            {
                Http = new()
                {
                    Method = "PUT"
                }
            },
            PathParameters = new Dictionary<string, string>
            {
                { "userId", userId }
            },
            Body = JsonSerializer.Serialize(updatedUserData)
        };
        
        var context = new TestLambdaContext();
        var function = new Function(mockRepo.Object);
        
        // Act
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(userId, response.Body); // Should contain the user ID in the error message
        
        // Verify the repository was called but update was not attempted
        mockRepo.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
