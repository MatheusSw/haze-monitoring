using System;
using System.Globalization;

namespace HazeMonitoring.models;

public static class MeasurementFactory
{
    public static IMeasurement Make(string clusterId, MeasurementCreateRequest measurementCreateRequest)
    {
        return new Measurement
        {
            ClusterId = clusterId,
            Reading = measurementCreateRequest.Reading,
            Timestamp = measurementCreateRequest.Timestamp.ToString("O", CultureInfo.InvariantCulture),
            Type = measurementCreateRequest.Type
        };
    }
}