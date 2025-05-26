using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace AtomManagementFunction.Tests;

public class FunctionTests
{
    [Fact]
    public async Task TestFunctionHandler()
    {
        var request = new APIGatewayHttpApiV2ProxyRequest();
        var context = new TestLambdaContext();
        var function = new AtomManagementFunction.Function();

        var response = await function.FunctionHandler(request, context);
        
        Assert.Equal(200, response.StatusCode);
        Assert.Contains("AtomManagementFunction is working!", response.Body);
    }
}
