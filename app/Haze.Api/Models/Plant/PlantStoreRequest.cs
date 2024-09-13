using System.Text.Json.Serialization;

namespace HazeApi.Models.Plant;

public class PlantStoreRequest
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("strain")] public string Strain { get; set; }
    [JsonPropertyName("stage")] public string Stage { get; set; }
    [JsonPropertyName("weight")] public decimal? Weight { get; set; }
}