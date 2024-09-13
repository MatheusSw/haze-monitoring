using Haze.Infra.Models.Clusters;

namespace Haze.Infra.Repositories;

public interface IClustersRepository
{
    /// <summary>
    /// Retrieves a cluster by the given id
    /// </summary>
    /// <param name="id">Cluster id to be fetched</param>
    /// <returns>The cluster if found; otherwise default</returns>
    public Task<Cluster?> Fetch(string id);

    /// <summary>
    /// Retrieves all created clusters
    /// </summary>
    /// <returns>All found clusters; or empty if no clusters were found</returns>
    public Task<IEnumerable<Cluster>> Fetch();

    /// <summary>
    /// Updates a cluster by the given id
    /// </summary>
    /// <param name="cluster">The content to be updated on the cluster</param>
    /// <returns>The newly updated cluster or null</returns>
    public Task<Cluster?> Update(Cluster cluster);

    /// <summary>
    /// Creates a new cluster
    /// </summary>
    /// <param name="cluster">The cluster to be created</param>
    /// <returns>The newly created cluster or default if the operation failed</returns>
    public Cluster? Create(Cluster cluster);
}