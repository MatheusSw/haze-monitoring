using Haze.Application.Commands.Clusters;
using Haze.Infra.Models.Clusters;
using Haze.Infra.Repositories;
using Serilog;
using Serilog.Context;

namespace Haze.Application.UseCases.Clusters;

public class ClusterCommandHandler(IClustersRepository clustersRepository) : IClusterCommandHandler
{
    //TODO: Should return an application/domain model
    public Cluster? Handle(CreateClusterCommand createClusterCommand)
    {
        LogContext.PushProperty("Command", createClusterCommand, true);

        var cluster = new Cluster
        {
            Name = createClusterCommand.Name,
            Location = createClusterCommand.Location,
            Id = Guid.NewGuid().ToString()
        };

        LogContext.PushProperty("Cluster request", cluster, true);

        var result = clustersRepository.Create(cluster);

        if (result is null)
        {
            Log.Logger
                .Warning("It was not possible to create a new cluster");

            return default;
        }

        Log.Logger
            .ForContext("Created Cluster", result, true)
            .Information("Cluster was successfully created");

        return result;
    }

    public async Task<Cluster?> Handle(UpdateClusterCommand updateClusterCommand)
    {
        LogContext.PushProperty("Command", updateClusterCommand, true);
        
        var cluster = new Cluster
        {
            Name = updateClusterCommand.Name,
            Id = updateClusterCommand.Id,
            Location = updateClusterCommand.Location
        };

        LogContext.PushProperty("Cluster request", cluster, true);
        
        var result = await clustersRepository.Update(cluster);

        if (result is null)
        {
            Log.Logger
                .Warning("It was not possible to update the cluster");

            return default;
        }

        Log.Logger
            .ForContext("Created Cluster", result, true)
            .Information("Cluster was successfully updated");

        return result;
    }
}