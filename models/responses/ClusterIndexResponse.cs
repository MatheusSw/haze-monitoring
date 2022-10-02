using System;
using System.Globalization;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using HazeMonitoring.models.dynamodb;

namespace HazeMonitoring.models.responses;

public class ClusterIndexResponse
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("location")] public string Location { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("state")] public string? State { get; set; }

    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }

    public ClusterIndexResponse(ClusterDbModel clusterDbModel)
    {
        Id = clusterDbModel.HashKey.Substring(clusterDbModel.HashKey.IndexOf('-') + 1);
        
        var createdAtSuccess = DateTime.TryParse(clusterDbModel.CreatedAt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
            out var createdAtConverted);
        CreatedAt = createdAtSuccess ? createdAtConverted : default;
        
        Location = clusterDbModel.Location;
        Name = clusterDbModel.Name;
        State = clusterDbModel.State;
        
        var updatedAtSuccess = DateTime.TryParse(clusterDbModel.UpdatedAt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
            out var updateAtConverted);
        UpdatedAt = updatedAtSuccess ? updateAtConverted : default;
    }
}