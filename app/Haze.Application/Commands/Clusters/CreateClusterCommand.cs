namespace Haze.Application.Commands.Clusters;

public class CreateClusterCommand
{
    public required string Name { get; set; }
    public string? Location { get; set; }
}