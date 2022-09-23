using System.Text.Json.Serialization;

namespace HazeMonitoring.models;

public class Measurement : IMeasurement
{
    [JsonPropertyName("cluster_id")]
    public string ClusterId { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("reading")]
    public decimal Reading { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}