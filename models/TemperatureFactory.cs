using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models;

public static class TemperatureFactory
{
    public static Document Make(string clusterId, decimal temperatureReading)
    {
        return new Document
        {
            ["PK"] = clusterId,
            ["SK"] = $"temperature-{Ulid.NewUlid().ToString()}",
            ["Reading"] = temperatureReading
        };
    }
}