using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HazeMonitoring.handlers;
using HazeMonitoring.models.factory;
using HazeMonitoring.models.requests;
using HazeMonitoring.models.responses;

// ReSharper disable PossibleInvalidOperationException
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HazeMonitoring.dispatchers;

public class MeasurementDispatcher
{
    private readonly AmazonSimpleNotificationServiceClient _notificationService = new(region: Amazon.RegionEndpoint.SAEast1);
    private readonly string _snsMeasurementsTopicArn = Environment.GetEnvironmentVariable("hazeMeasurementsTopicArn");

    //todo implement correlation id logging for easier tracing
    public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);
            var measurementCreateRequest = JsonSerializer.Deserialize<MeasurementCreateRequest>(gatewayRequest.Body);

            var measurement = MeasurementFactory.Make(clusterId, measurementCreateRequest);
            
            if (await ClustersHandler.Details(context.Logger, measurement.ClusterId) is null)
            {
                context.Logger.LogWarning($"Insertion of measurement for non existing cluster was blocked - {JsonSerializer.Serialize(measurement)}");
                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(measurementCreateRequest),
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            context.Logger.LogInformation($"Publishing request to SNS - {JsonSerializer.Serialize(measurement)}");
            var notificationRequest = new PublishRequest
            {
                Message = JsonSerializer.Serialize(measurement),
                TopicArn = _snsMeasurementsTopicArn
            };

            var notificationResponse = await _notificationService.PublishAsync(notificationRequest);
            context.Logger.LogInformation($"Received response from SNS notification - {JsonSerializer.Serialize(notificationResponse)}");
            
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(measurementCreateRequest),
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
    
    //todo implement correlation id logging for easier tracing
    public async Task<APIGatewayProxyResponse> Index(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
    {
        try
        {
            _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);

            var measurements = await MeasurementsHandler.Index(clusterId, context.Logger);
            var measurementsResponse = measurements.Select(m => new MeasurementsIndexResponse(m)).ToList();
            
            if (!measurementsResponse.Any())
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(measurementsResponse),
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
}