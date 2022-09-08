using System.Text.Json.Serialization;

namespace HazeMonitoring.models;

public class HumidityCreateRequest
{
    [JsonPropertyName("humidity")] public decimal? Humidity { get; set; }
}