using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models;

public static class MonitoringDocumentFactory
{
    public static Document Make(IMeasurement measurement)
    {
        return new Document
        {
            ["PK"] = ClusterDocumentFactory.GeneratePrimaryKeyFromClusterId(measurement.ClusterId),
            ["SK"] = $"measurement-{measurement.Type}#{measurement.Timestamp}",
            ["Measurement-reading"] = measurement.Reading
        };
    }
}