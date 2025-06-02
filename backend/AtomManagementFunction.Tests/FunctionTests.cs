using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Common.Requests.AtomRequests;

namespace AtomManagementFunction.Tests;

public class FunctionTests
{
    [Fact]
    public async Task TestFunctionHandler()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "ap-southeast-1");

        var request = new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = "GET",
                    Path = "/atoms/17065c63-90eb-4933-a874-1f120a6f454b"
                }
            },
            Headers = new Dictionary<string, string>
            {
                { "x-user-id", "e5f6g7h8-i9j0-1234-efgh-567890123113" }
            }
        };
        var context = new TestLambdaContext();
        var function = new AtomManagementFunction.Function();

        var response = await function.FunctionHandler(request, context);

        Console.WriteLine("Response body: " + response.Body);

        // Assert.Equal(201, response.StatusCode);
        // var json = JObject.Parse(response.Body);
        // Assert.True(json["Success"]?.Value<bool>() == true);
        // Assert.Equal("Atoms retrieved successfully", json["Message"]?.Value<string>());
    }
}
