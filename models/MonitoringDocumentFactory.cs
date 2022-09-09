using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models;

public static class MonitoringDocumentFactory
{
    public static Document Make(IMeasurement measurement)
    {
        return new Document
        {
            ["PK"] = measurement.ClusterId,
            ["SK"] = measurement.TypeTimestamp,
            ["Reading"] = measurement.MeasureValue
        };
    }
}