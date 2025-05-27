using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Newtonsoft.Json;
using NeuroBrain.Common.Repositories;
using NeuroBrain.NoteManagementFunction.Services;
using NeuroBrain.NoteManagementFunction.Handlers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

//test
//test
namespace NeuroBrain.NoteManagementFunction
{
    public class Function
    {
        private readonly INoteRepository _noteRepository;
        private readonly NoteService _noteService;

        public Function()
        {
            var dynamoDb = new AmazonDynamoDBClient();
            _noteRepository = new NoteRepository(dynamoDb);
            _noteService = new NoteService(_noteRepository);
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var method = request.RequestContext.Http.Method.ToUpper();
                var path = request.RequestContext.Http.Path;
                var userId = GetUserIdFromClaims(request);

                return method switch
                {
                    "POST" when path.EndsWith("/notes") => await new CreateNoteHandler(_noteService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/notes") => await new GetNotesHandler(_noteService).HandleAsync(request, userId),
                    "GET" when path.Contains("/notes/") && !path.EndsWith("/search") && !path.EndsWith("/tags") => await new GetNoteByIdHandler(_noteService).HandleAsync(request, userId),
                    "PUT" when path.Contains("/notes/") && !path.Contains("/archive") && !path.Contains("/restore") => await new UpdateNoteHandler(_noteService).HandleAsync(request, userId),
                    "DELETE" when path.Contains("/notes/") => await new DeleteNoteHandler(_noteService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/notes/search") => await new SearchNotesHandler(_noteService).HandleAsync(request, userId),
                    "PUT" when path.Contains("/archive") => await new ArchiveNoteHandler(_noteService).HandleAsync(request, userId),
                    "PUT" when path.Contains("/restore") => await new RestoreNoteHandler(_noteService).HandleAsync(request, userId),
                    "GET" when path.EndsWith("/notes/tags") => await new GetNoteTagsHandler(_noteService).HandleAsync(request, userId),
                    _ => new APIGatewayHttpApiV2ProxyResponse
                    {
                        StatusCode = 404,
                        Body = JsonConvert.SerializeObject(new { message = "Endpoint not found" }),
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonConvert.SerializeObject(new { message = "Internal server error", error = ex.Message }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }

        private string GetUserIdFromClaims(APIGatewayHttpApiV2ProxyRequest request)
        {
            // Extract user ID from JWT claims or headers
            if (request.Headers?.ContainsKey("authorization") == true)
            {
                // In production, decode JWT token and extract userId
                // For now, return a placeholder
                return "user-123";
            }

            // Check for custom user header
            if (request.Headers?.ContainsKey("x-user-id") == true)
            {
                return request.Headers["x-user-id"];
            }

            // Default fallback
            return "user-123";
        }
    }
}
