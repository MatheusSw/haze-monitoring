using System.Data.Entity;
using Haze.Infra.Database;
using Haze.Infra.Models.Clusters;
using Serilog;

namespace Haze.Infra.Repositories;

public class ClustersRepository(HazeContext hazeContext) : IClustersRepository
{
    public async Task<Cluster?> Fetch(string id)
    {
        Log.Logger.Information("Initiating fetch by cluster id {id}", id); //TODO: Level=Debug

        var result = await hazeContext.Clusters.FindAsync(id);

        Log.Logger
            .ForContext("Cluster", result, result is not null)
            .Information("Cluster fetch result");

        return result;
    }

    public async Task<IEnumerable<Cluster>> Fetch()
    {
        Log.Logger.Information("Initiating all fetch"); //TODO: Level=Debug

        var result = await hazeContext.Clusters.ToListAsync();

        Log.Logger
            .ForContext("Clusters", result, true)
            .Information("All clusters fetch result");

        return result;
    }

    public async Task<Cluster?> Update(Cluster cluster)
    {
        Log.Logger
            .ForContext("Request", cluster, true)
            .Information("Initiating cluster update request");

        var entity = await Fetch(cluster.Id);
        if (entity is null)
        {
            Log.Logger
                .ForContext("Cluster request", cluster, true)
                .Error("Unable to update entity, it was not found in the database");

            return default;
        }

        Log.Logger
            .ForContext("Existing cluster", entity, true)
            .Information("Existing cluster to be updated found");

        hazeContext.Entry(entity).CurrentValues.SetValues(cluster);

        try
        {
            await hazeContext.SaveChangesAsync();

            Log.Logger
                .Information("Cluster updated successfully");

            return cluster; //TODO: This is weird
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error ocurred while trying to update a cluster");

            return default;
        }
    }

    public Cluster? Create(Cluster cluster)
    {
        Log.Logger
            .ForContext("Request", cluster, true)
            .Information("Creating new cluster");

        try
        {
            var result = hazeContext.Clusters.Add(cluster);
            hazeContext.SaveChanges();

            Log.Logger
                .ForContext("Result", result.Entity, true)
                .Information("New cluster created successfully");

            return result.Entity;
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "Failed to create cluster");

            return default;
        }
    }
}