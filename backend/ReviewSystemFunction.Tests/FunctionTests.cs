using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace ReviewSystemFunction.Tests;

public class FunctionTests
{
    [Fact]
    public async Task TestFunctionHandler()
    {
        var request = new APIGatewayHttpApiV2ProxyRequest();
        var context = new TestLambdaContext();
        var function = new ReviewSystemFunction.Function();

        var response = await function.FunctionHandler(request, context);
        
        Assert.Equal(200, response.StatusCode);
        Assert.Contains("ReviewSystemFunction is working!", response.Body);
    }
}
