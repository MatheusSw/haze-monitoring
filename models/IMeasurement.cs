using System.Text.Json.Serialization;

namespace HazeMonitoring.models;

public interface IMeasurement
{
    [JsonPropertyName("cluster_id")]
    string ClusterId { get; set; }
    
    [JsonPropertyName("type_timestamp")]
    string TypeTimestamp { get; set; }
    
    [JsonPropertyName("measurement")]
    decimal MeasureValue { get; set; }
}