using Haze.Application.Commands.Clusters;
using Haze.Infra.Models.Clusters;

namespace Haze.Application.UseCases.Clusters;

public interface IClusterCommandHandler
{
    Cluster? Handle(CreateClusterCommand createClusterCommand);
    Task<Cluster?> Handle(UpdateClusterCommand updateClusterCommand);
}