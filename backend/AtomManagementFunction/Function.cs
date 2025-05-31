using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Common.Repositories;
using NeuroBrain.AtomManagementFunction.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AtomManagementFunction;

public class Function
{

    private readonly IAtomRepository _atomRepository;
    private readonly AtomService _atomService;

    public Function()
    {
        var dynamoDb = new AmazonDynamoDBClient();
        _atomRepository = new AtomRepository(dynamoDb);
        _atomService = new AtomService(_atomRepository);
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request in AtomManagementFunction");

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { Message = "AtomManagementFunction is working!" }),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
