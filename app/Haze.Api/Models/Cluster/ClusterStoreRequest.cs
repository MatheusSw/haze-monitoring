using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace HazeApi.Models.Cluster;

public class ClusterStoreRequest
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("location")] public string? Location { get; set; }
}