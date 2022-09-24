using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using HazeMonitoring.models;
using HazeMonitoring.models.document_factory;

namespace HazeMonitoring.handlers;

public static class PlantsHandler
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new();
    private static readonly string MonitoringTableName = Environment.GetEnvironmentVariable("hazeMonitoringTableName");

    public static async Task Insert(Plant plant, ILambdaLogger logger)
    {
        try
        {
            var table = Table.LoadTable(DynamoDbClient, MonitoringTableName);

            var plantDocument = PlantDocumentFactory.Make(plant);
            _ = await table.PutItemAsync(plantDocument);
        }
        catch (Exception)
        {
            logger.LogError(
                $"There has been an error while trying to process the plant - {JsonSerializer.Serialize(plant)}");
            throw;
        }
        await Task.CompletedTask;
    }
}