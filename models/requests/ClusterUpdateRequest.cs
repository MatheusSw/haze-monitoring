using System.Text.Json.Serialization;

namespace HazeMonitoring.models.requests;

public class ClusterUpdateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("state")]
    public string State { get; set; }
    [JsonPropertyName("location")]
    public string Location { get; set; }
}