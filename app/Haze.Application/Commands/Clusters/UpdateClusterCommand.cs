namespace Haze.Application.Commands.Clusters;

public class UpdateClusterCommand
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Location { get; set; }
}