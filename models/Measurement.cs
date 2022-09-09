using System.Text.Json.Serialization;

namespace HazeMonitoring.models;

public class Measurement : IMeasurement
{
    [JsonPropertyName("cluster_id")]
    public string ClusterId { get; set; }
    
    [JsonPropertyName("type_timestamp")]
    public string TypeTimestamp { get; set; }
    
    [JsonPropertyName("measurement")]
    public decimal MeasureValue { get; set; }
}