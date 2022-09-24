using System;
using System.Text.Json.Serialization;

namespace HazeMonitoring.models.requests;

public class MeasurementCreateRequest
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("reading")] public decimal Reading { get; set; }
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}