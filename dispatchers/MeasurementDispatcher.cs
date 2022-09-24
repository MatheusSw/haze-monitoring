using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HazeMonitoring.models;
using HazeMonitoring.models.requests;

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
}