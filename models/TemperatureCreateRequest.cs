using System.Text.Json.Serialization;

namespace HazeMonitoring.models;

public class TemperatureCreateRequest
{
    [JsonPropertyName("temperature")] public decimal? Temperature { get; set; }
}