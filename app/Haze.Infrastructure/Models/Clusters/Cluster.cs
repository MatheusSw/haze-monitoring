using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haze.Infra.Models.Clusters;

[Table("Clusters")]
public class Cluster
{
    [MaxLength(255)] [Column("id")] public required string Id { get; set; }
    [MaxLength(255)] [Column("name")] public required string Name { get; set; }
    [MaxLength(255)] [Column("location")] public string? Location { get; set; }
}