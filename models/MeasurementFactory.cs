using System;
using System.Globalization;

namespace HazeMonitoring.models;

public static class MeasurementFactory
{
    public enum MeasurementType
    {
        Humidity,
        Temperature
    }
    
    public static IMeasurement Make(string clusterId, MeasurementType type, decimal measurement)
    {
        return new Measurement
        {
            MeasureValue = measurement,
            ClusterId = clusterId,
            TypeTimestamp = $"{type.ToString()}-{DateTime.UtcNow.ToString("O", CultureInfo.CurrentCulture)}"
        };
    }
}