using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HazeMonitoring.handlers;
using HazeMonitoring.models.factory;
using HazeMonitoring.models.requests;
using HazeMonitoring.models.responses;

namespace HazeMonitoring.dispatchers;

public class ClustersDispatcher
{
    //todo implement correlation id logging for easier tracing
    public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            var clusterCreateRequest = JsonSerializer.Deserialize<ClusterCreateRequest>(gatewayRequest.Body);

            context.Logger.LogInformation(
                $"Received new cluster request - {JsonSerializer.Serialize(clusterCreateRequest)}");
            
            if (string.IsNullOrEmpty(clusterCreateRequest.Location) || string.IsNullOrEmpty(clusterCreateRequest.Name))
            {
                context.Logger.LogInformation("Cluster create request did not contain all necessary data");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }
            
            var cluster = ClusterFactory.Make(clusterCreateRequest);
            context.Logger.LogInformation($"Cluster model created - {JsonSerializer.Serialize(cluster)}");

            await ClustersHandler.Insert(cluster, context.Logger);

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(clusterCreateRequest),
                StatusCode = (int) HttpStatusCode.Accepted,
                Headers = new Dictionary<string, string>
                {
                    {"Access-Control-Allow-Origin", "*"},
                    {"Access-Control-Allow-Credentials", "true"}
                }
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

    public async Task<APIGatewayProxyResponse> Details(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);

            context.Logger.LogInformation($"Received cluster details request - Cluster id = {clusterId}");

            var cluster = await ClustersHandler.Details(context.Logger, clusterId);

            if (cluster == default)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(cluster),
                StatusCode = (int) HttpStatusCode.OK
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
    
    public async Task<APIGatewayProxyResponse> Index(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation("Received cluster index request");

            var clusters = await ClustersHandler.Index(context.Logger);
            var clustersResponse = clusters.Select(c => new ClusterIndexResponse(c)).ToList();
            
            if (!clustersResponse.Any())
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(clustersResponse.ToList()),
                StatusCode = (int) HttpStatusCode.OK,
                Headers = new Dictionary<string, string>
                {
                    {"Access-Control-Allow-Origin", "*"},
                    {"Access-Control-Allow-Credentials", "true"}
                }
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
    
    public async Task<APIGatewayProxyResponse> Update(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);

            context.Logger.LogInformation($"Received cluster update request - Cluster id = {clusterId}");

            var clusterUpdateRequest = JsonSerializer.Deserialize<ClusterUpdateRequest>(gatewayRequest.Body);

            //TODO Use Cluster model instead of ClusterUpdateRequest
            var cluster = await ClustersHandler.Update(context.Logger, clusterId, clusterUpdateRequest);

            if (cluster == default)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(cluster),
                StatusCode = (int) HttpStatusCode.OK
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