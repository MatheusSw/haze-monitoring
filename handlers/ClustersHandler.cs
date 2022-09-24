using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using HazeMonitoring.models;
using HazeMonitoring.models.document_factory;

namespace HazeMonitoring.handlers;

public static class ClustersHandler
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new();
    private static readonly string MonitoringTableName = Environment.GetEnvironmentVariable("hazeMonitoringTableName");

    public static async Task Insert(Cluster cluster, ILambdaLogger logger)
    {
        try
        {
            var table = Table.LoadTable(DynamoDbClient, MonitoringTableName);

            var clusterDocument = ClusterDocumentFactory.Make(cluster);
            
            _ = await table.PutItemAsync(clusterDocument);
        }
        catch (Exception)
        {
            logger.LogError(
                $"There has been an error while trying to process the cluster - {JsonSerializer.Serialize(cluster)}");
            throw;
        }
        await Task.CompletedTask;
    }
}