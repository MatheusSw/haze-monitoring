using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using HazeMonitoring.models;

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
            context.Logger.LogInformation($"Processing a new measurement - {JsonSerializer.Serialize(record.Sns)}");
            var measurement = JsonSerializer.Deserialize<Measurement>(record.Sns.Message);
            
            var table = Table.LoadTable(DynamoDbClient, MonitoringTableName);

            var monitoringDocument = MonitoringDocumentFactory.Make(measurement);
            
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
}