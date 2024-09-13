namespace Haze.Domain.Entities.Plants;

public class Plant(string? name, string strain, PlantStage stage, decimal? weight)
{
    public string? Name { get; set; } = name;
    public string Strain { get; set; } = strain;
    public PlantStage Stage { get; set; } = stage;
    public decimal? Weight { get; set; } = weight;
}