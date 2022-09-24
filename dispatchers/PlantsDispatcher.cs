using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HazeMonitoring.handlers;
using HazeMonitoring.models;
using HazeMonitoring.models.requests;

// ReSharper disable PossibleInvalidOperationException

namespace HazeMonitoring.dispatchers;

public class PlantsDispatcher
{
    //todo implement correlation id logging for easier tracing
    public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);
            var plantCreateRequest = JsonSerializer.Deserialize<PlantCreateRequest>(gatewayRequest.Body);

            context.Logger.LogInformation($"Received new plant request - {JsonSerializer.Serialize(plantCreateRequest)}");
            var plant = PlantFactory.Make(clusterId, plantCreateRequest);
            context.Logger.LogInformation($"Plant model created - {JsonSerializer.Serialize(plant)}");

            await PlantsHandler.Insert(plant, context.Logger);

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(plantCreateRequest),
                StatusCode = (int) HttpStatusCode.Accepted
            };
        }
        catch (Exception e)
        {
            context.Logger.LogError($"An error ocurred while processing the request - {e.Message} - {e.StackTrace}");
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }
}