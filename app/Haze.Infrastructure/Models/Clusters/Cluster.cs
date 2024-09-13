using System.ComponentModel.DataAnnotations.Schema;

namespace Haze.Infra.Models.Clusters;

[Table("clusters")]
public class Cluster
{
    public required string Id { get; set; }
    [Column("name")] public required string Name { get; set; }
    [Column("location")] public string? Location { get; set; }
}