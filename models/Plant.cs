#nullable enable
namespace HazeMonitoring.models;

public class Plant
{
    public string Id { get; set; }
    public string ClusterId { get; set; }
    public string Strain { get; set; }
    public ulong Lifetime { get; set; }
    public string State { get; set; }
    public string? ClonedFrom { get; set; }
}