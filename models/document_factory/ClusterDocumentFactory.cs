using System;
using System.Globalization;
using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models.document_factory;

public static class ClusterDocumentFactory
{
    public static Document Make(Cluster cluster)
    {
        var clusterId = Guid.NewGuid().ToString();
        return new Document
        {
            ["PK"] = GeneratePrimaryKeyFromClusterId(clusterId),
            ["SK"] = GeneratePrimaryKeyFromClusterId(clusterId),
            ["Cluster-name"] = cluster.Name,
            ["Cluster-state"] = cluster.State,
            ["Cluster-location"] = cluster.Location,
            ["Cluster-createdAt"] = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
            ["Cluster-updatedAt"] = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
        };
    }
    
    public static string GeneratePrimaryKeyFromClusterId(string clusterId)
    {
        return $"cluster-{clusterId}";
    }
}