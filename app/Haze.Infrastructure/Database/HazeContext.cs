using Haze.Infra.Models.Clusters;
using Microsoft.EntityFrameworkCore;

namespace Haze.Infra.Database;

public class HazeContext : DbContext
{
    public HazeContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Cluster> Clusters { get; set; }
}