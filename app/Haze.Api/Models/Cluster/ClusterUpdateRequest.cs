using System.Text.Json.Serialization;

namespace HazeApi.Models.Cluster;

public class ClusterUpdateRequest
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("location")] public string Location { get; set; }
}