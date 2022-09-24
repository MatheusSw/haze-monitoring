namespace HazeMonitoring.models.document_factory;

public static class ClusterDocumentFactory
{
    public static string GeneratePrimaryKeyFromClusterId(string clusterId)
    {
        return $"cluster-{clusterId}";
    }
}