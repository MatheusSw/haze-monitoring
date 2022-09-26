using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using HazeMonitoring.models;
using HazeMonitoring.models.document_factory;
using HazeMonitoring.models.dynamodb;

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

    public static async Task<ClusterDbModel> Details(ILambdaLogger logger, string clusterId)
    {
        try
        {
            var dynamoDbContext = new DynamoDBContext(DynamoDbClient);
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = MonitoringTableName
            };

            var clusterPrimaryKey = ClusterDocumentFactory.GeneratePartitionKeyFromClusterId(clusterId);
            var cluster = await
                dynamoDbContext.LoadAsync<ClusterDbModel>(clusterPrimaryKey, clusterPrimaryKey, config);
            
            if (cluster is null)
            {
                logger.LogInformation($"Cluster not found - id {clusterId}");
            }

            logger.LogInformation($"Cluster found - {JsonSerializer.Serialize(cluster)}");

            return cluster;
        }
        catch (Exception)
        {
            logger.LogError(
                $"There has been an error while trying to search details for the cluster - {clusterId}");
            throw;
        }

        await Task.CompletedTask;
    }
}