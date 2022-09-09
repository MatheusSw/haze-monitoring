using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models;

public static class HumidityFactory
{
    public static Document Make(string clusterId, decimal humidityReading)
    {
        return new Document
        {
            ["PK"] = clusterId,
            ["SK"] = $"humidity-{Ulid.NewUlid().ToString()}",
            ["Reading"] = humidityReading
        };
    }
}