using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HazeMonitoring.models;

[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HazeMonitoring.handlers
{
    public class TemperatureHandler
    {
        private readonly AmazonSimpleNotificationServiceClient _notificationService = new(region: Amazon.RegionEndpoint.SAEast1);
        private readonly string _snsMeasurementsTopicArn = Environment.GetEnvironmentVariable("hazeMeasurementsTopicArn");

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

        public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest gatewayRequest, ILambdaContext context)
        {
            try
            {
                _ = gatewayRequest.PathParameters.TryGetValue("cluster-id", out var clusterId);
                var temperatureCreateRequest = JsonSerializer.Deserialize<TemperatureCreateRequest>(gatewayRequest.Body);

                var measurement = MeasurementFactory.Make(clusterId, MeasurementFactory.MeasurementType.Temperature,
                    temperatureCreateRequest!.Temperature.Value);
            
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
                    Body = JsonSerializer.Serialize(temperatureCreateRequest),
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
}