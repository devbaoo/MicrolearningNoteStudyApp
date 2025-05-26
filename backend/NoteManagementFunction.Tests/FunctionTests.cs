using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace NoteManagementFunction.Tests;

public class FunctionTests
{
    [Fact]
    public async Task TestFunctionHandler()
    {
        var request = new APIGatewayHttpApiV2ProxyRequest();
        var context = new TestLambdaContext();
        var function = new NoteManagementFunction.Function();

        var response = await function.FunctionHandler(request, context);
        
        Assert.Equal(200, response.StatusCode);
        Assert.Contains("NoteManagementFunction is working!", response.Body);
    }
}
