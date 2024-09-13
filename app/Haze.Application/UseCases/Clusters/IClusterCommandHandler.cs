using Haze.Application.Commands.Clusters;
using Haze.Infra.Models.Clusters;

namespace Haze.Application.UseCases.Clusters;

public interface IClusterCommandHandler
{
    Cluster? Handle(CreateClusterCommand command);
    Task<Cluster?> Handle(UpdateClusterCommand command);
    Task<Cluster?> Handle(FetchClusterCommand command);
}