namespace HazeMonitoring.models;

public sealed class Cluster
{
    public string Name { get; set; }
    public string? State { get; set; }
    public string Location { get; set; }
}