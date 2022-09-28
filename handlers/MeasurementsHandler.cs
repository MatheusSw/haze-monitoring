using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using HazeMonitoring.models;
using HazeMonitoring.models.document_factory;
using HazeMonitoring.models.dynamodb;

namespace HazeMonitoring.handlers;

public class MeasurementsHandler
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new();
    private static readonly string MonitoringTableName = Environment.GetEnvironmentVariable("hazeMonitoringTableName");

    public async Task Handle(
        SNSEvent evnt, ILambdaContext context)
    {
        foreach (var record in evnt.Records)
        {
            await ProcessRecordAsync(record, context);
        }
    }

    private async Task ProcessRecordAsync(
        SNSEvent.SNSRecord record, ILambdaContext context)
    {
        try
        {
            //TODO check if the cluster actually exists before inserting measurement
            context.Logger.LogInformation($"Processing a new measurement - {JsonSerializer.Serialize(record.Sns)}");
            var measurement = JsonSerializer.Deserialize<Measurement>(record.Sns.Message);

            var table = Table.LoadTable(DynamoDbClient, MonitoringTableName);

            var monitoringDocument = MeasurementDocumentFactory.Make(measurement);

            _ = await table.PutItemAsync(monitoringDocument);
        }
        catch (Exception)
        {
            context.Logger.LogError(
                $"There has been an error while trying to process the sns record - {JsonSerializer.Serialize(record.Sns)}");
            throw;
        }

        await Task.CompletedTask;
    }

    public static async Task<IEnumerable<MeasurementDbModel>> Index(string clusterId, ILambdaLogger logger)
    {
        try
        {
            var dynamoDbContext = new DynamoDBContext(DynamoDbClient);
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = MonitoringTableName,
                BackwardQuery = true
            };

            //TODO FIX THIS!!!!!!!!!!!! DynamoDb remodel is necessary
            var dateTimeNowUtc = DateTime.UtcNow;
            var measurements = new List<MeasurementDbModel>();

            var measurementsHumiditySearch =
                dynamoDbContext.QueryAsync<MeasurementDbModel>(
                    ClusterDocumentFactory.GeneratePartitionKeyFromClusterId(clusterId), QueryOperator.Between,
                    new[]
                    {
                        $"measurement-humidity#{dateTimeNowUtc.Subtract(new TimeSpan(0, 5, 0)):O}",
                        $"measurement-humidity#{dateTimeNowUtc:O}"
                    }, config);

            do
            {
                var measurementsSet = await measurementsHumiditySearch.GetNextSetAsync();
                measurements.AddRange(measurementsSet);
            } while (!measurementsHumiditySearch.IsDone);

            var measurementsTemperatureSearch =
                dynamoDbContext.QueryAsync<MeasurementDbModel>(
                    ClusterDocumentFactory.GeneratePartitionKeyFromClusterId(clusterId), QueryOperator.Between,
                    new[]
                    {
                        $"measurement-temperature#{dateTimeNowUtc.Subtract(new TimeSpan(0, 5, 0)):O}",
                        $"measurement-temperature#{dateTimeNowUtc:O}"
                    }, config);

            do
            {
                var measurementsSet = await measurementsTemperatureSearch.GetNextSetAsync();
                measurements.AddRange(measurementsSet);
            } while (!measurementsTemperatureSearch.IsDone);

            logger.LogInformation($"Measurements index retrieved - {JsonSerializer.Serialize(measurements)}");
            return measurements;
        }
        catch (Exception)
        {
            logger.LogError("There has been an error while trying to scan all clusters");
            throw;
        }
    }
}