using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HazeMonitoring.handlers;
using HazeMonitoring.models;
using HazeMonitoring.models.factory;
using HazeMonitoring.models.requests;

namespace HazeMonitoring.dispatchers;

public class ClustersDispatcher
{
    //todo implement correlation id logging for easier tracing
    public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            var clusterCreateRequest = JsonSerializer.Deserialize<ClusterCreateRequest>(gatewayRequest.Body);

            context.Logger.LogInformation($"Received new cluster request - {JsonSerializer.Serialize(clusterCreateRequest)}");
            var cluster = ClusterFactory.Make(clusterCreateRequest);
            context.Logger.LogInformation($"Cluster model created - {JsonSerializer.Serialize(cluster)}");

            await ClustersHandler.Insert(cluster, context.Logger);

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(clusterCreateRequest),
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