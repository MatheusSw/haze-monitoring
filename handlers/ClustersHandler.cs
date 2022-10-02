using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using HazeMonitoring.models;
using HazeMonitoring.models.document_factory;
using HazeMonitoring.models.dynamodb;
using HazeMonitoring.models.requests;

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

            logger.LogInformation(cluster is null
                ? $"Cluster not found - id {clusterId}"
                : $"Cluster found - {JsonSerializer.Serialize(cluster)}");

            return cluster;
        }
        catch (Exception e)
        {
            logger.LogError(
                $"There has been an error while trying to search details for the cluster - {clusterId} - {e.Message} - {e.StackTrace}");
            throw;
        }
    }

    public static async Task<IEnumerable<ClusterDbModel>> Index(ILambdaLogger logger)
    {
        try
        {
            var dynamoDbContext = new DynamoDBContext(DynamoDbClient);
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = MonitoringTableName
            };

            var clustersSearch = dynamoDbContext.ScanAsync<ClusterDbModel>(new List<ScanCondition>
            {
                new("HashKey", ScanOperator.BeginsWith, "cluster"),
                new("SortKey", ScanOperator.BeginsWith, "cluster")
            }, config);

            var clusters = new List<ClusterDbModel>();
            do
            {
                var clustersSet = await clustersSearch.GetNextSetAsync();
                clusters.AddRange(clustersSet);
            } while (!clustersSearch.IsDone);

            logger.LogInformation($"Clusters index retrieved - {JsonSerializer.Serialize(clusters)}");
            return clusters;
        }
        catch (Exception)
        {
            logger.LogError("There has been an error while trying to scan all clusters");
            throw;
        }
    }

    public static async Task<ClusterDbModel> Update(ILambdaLogger logger, string clusterId,
        ClusterUpdateRequest clusterUpdateRequest)
    {
        try
        {
            var dynamoDbContext = new DynamoDBContext(DynamoDbClient);
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = MonitoringTableName
            };

            //TODO Instead of returning, save the new cluster
            var cluster = await Details(logger, clusterId);
            if (cluster is null)
            {
                return null;
            }

            cluster.Location = clusterUpdateRequest.Location;
            cluster.Name = clusterUpdateRequest.Name;
            cluster.State = clusterUpdateRequest.State;
            cluster.UpdatedAt = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

            logger.LogInformation(
                $"Updating cluster from {JsonSerializer.Serialize(cluster)} to {JsonSerializer.Serialize(clusterUpdateRequest)}");

            await dynamoDbContext.SaveAsync(cluster, config);

            return cluster;
        }
        catch (Exception)
        {
            logger.LogError(
                $"There has been an error while trying to search details for the cluster - {clusterId}");
            throw;
        }
    }
}