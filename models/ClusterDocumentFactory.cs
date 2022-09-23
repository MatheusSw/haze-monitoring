namespace HazeMonitoring.models;

public class ClusterDocumentFactory
{
    public static string GeneratePrimaryKeyFromClusterId(string clusterId)
    {
        return $"cluster-{clusterId}";
    }
}