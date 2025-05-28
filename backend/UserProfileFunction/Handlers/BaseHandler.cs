using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserProfileFunction.Handlers
{
    public abstract class BaseHandler
    {
        protected readonly JsonSerializerOptions _jsonOptions;

        public BaseHandler()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            };
        }

        protected APIGatewayHttpApiV2ProxyResponse CreateResponse(HttpStatusCode statusCode, object body)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = JsonSerializer.Serialize(body, _jsonOptions),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        protected APIGatewayHttpApiV2ProxyResponse NotFound(string message)
        {
            return CreateResponse(HttpStatusCode.NotFound, new { Message = message });
        }

        protected APIGatewayHttpApiV2ProxyResponse BadRequest(string message)
        {
            return CreateResponse(HttpStatusCode.BadRequest, new { Message = message });
        }

        protected APIGatewayHttpApiV2ProxyResponse Ok(object data)
        {
            return CreateResponse(HttpStatusCode.OK, data);
        }

        protected APIGatewayHttpApiV2ProxyResponse Created(object data)
        {
            return CreateResponse(HttpStatusCode.Created, data);
        }

        protected T DeserializeBody<T>(string body)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(body, _jsonOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
} 