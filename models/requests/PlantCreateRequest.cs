#nullable enable
using System.Text.Json.Serialization;

namespace HazeMonitoring.models.requests;

public class PlantCreateRequest
{
    [JsonPropertyName("strain")] public string Strain { get; set; }
    [JsonPropertyName("lifetime")] public ulong Lifetime { get; set; }
    [JsonPropertyName("state")] public string State { get; set; }
    [JsonPropertyName("cloned_from")] public string? ClonedFrom { get; set; }
}