using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Util;
using HazeMonitoring.models;

namespace HazeMonitoring.handlers;

public class HumidityHandler
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
            
            var humidityRequest = JsonSerializer.Deserialize<HumidityCreateRequest>(request.Body);
            if (humidityRequest?.Humidity is null)
            {
                context.Logger.LogError("There has been a problem processing the request body");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }

            var humidity = HumidityFactory.Make(request.PathParameters["cluster-id"], humidityRequest.Humidity.Value);
            
            _ = await table.PutItemAsync(humidity);
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(humidityRequest),
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