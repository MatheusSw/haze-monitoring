using System;
using System.Globalization;
using HazeMonitoring.models.requests;

namespace HazeMonitoring.models.factory;

public static class MeasurementFactory
{
    public static IMeasurement Make(string clusterId, MeasurementCreateRequest measurementCreateRequest)
    {
        return new Measurement
        {
            ClusterId = clusterId,
            Reading = measurementCreateRequest.Reading,
            Timestamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
            Type = measurementCreateRequest.Type
        };
    }
}