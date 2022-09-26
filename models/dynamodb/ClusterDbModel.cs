using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace HazeMonitoring.models.dynamodb;

public sealed class ClusterDbModel
{
    [DynamoDBHashKey("PK")]
    [JsonPropertyName("id")]
    public string HashKey { get; set; }

    [DynamoDBRangeKey("SK")] [JsonIgnore] public string SortKey { get; set; }

    [DynamoDBProperty("Cluster-createdAt")]
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [DynamoDBProperty("Cluster-location")]
    [JsonPropertyName("location")]
    public string Location { get; set; }

    [DynamoDBProperty("Cluster-name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [DynamoDBProperty("Cluster-state")]
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [DynamoDBProperty("Cluster-updatedAt")]
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; }
}