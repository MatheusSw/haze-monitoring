using HazeMonitoring.models.requests;

namespace HazeMonitoring.models.factory;

public static class ClusterFactory
{
    public static Cluster Make(ClusterCreateRequest clusterCreateRequest)
    {
        return new Cluster
        {
            Location = clusterCreateRequest.Location,
            Name = clusterCreateRequest.Name,
            State = clusterCreateRequest.State
        };
    }
}