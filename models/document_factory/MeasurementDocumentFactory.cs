using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models.document_factory;

public static class MeasurementDocumentFactory
{
    public static Document Make(IMeasurement measurement)
    {
        return new Document
        {
            ["PK"] = ClusterDocumentFactory.GeneratePartitionKeyFromClusterId(measurement.ClusterId),
            ["SK"] = $"measurement-{measurement.Type}#{measurement.Timestamp}",
            ["Measurement-reading"] = measurement.Reading
        };
    }
}