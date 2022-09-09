using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HazeMonitoring.models;

[assembly: LambdaSerializer(
typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace HazeMonitoring.handlers
{
    public class TemperatureHandler
    {
        private static readonly AmazonDynamoDBClient DynamoDbClient = new();
        private static readonly string MonitoringTableName = $"haze-monitoring-{Utils.GetApplicationStage()}-table";

        public APIGatewayProxyResponse Index(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Access-Control-Allow-Origin", "*"}
                }
            };
            return response;
        }
        
        public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var table = Table.LoadTable(DynamoDbClient, MonitoringTableName);
            
                var temperatureCreateRequest = JsonSerializer.Deserialize<TemperatureCreateRequest>(request.Body);
                if (temperatureCreateRequest?.Temperature is null)
                {
                    context.Logger.LogError("There has been a problem processing the request body");
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = (int) HttpStatusCode.BadRequest
                    };
                }

                var temperature = TemperatureFactory.Make(request.PathParameters["cluster-id"], temperatureCreateRequest.Temperature.Value);
            
                _ = await table.PutItemAsync(temperature);
                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(temperatureCreateRequest),
                    StatusCode = (int) HttpStatusCode.Created
                };
            }
            catch (Exception e)
            {
                context.Logger.LogError($"An error ocurred while processing the request - {e.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }
        }
    }
}