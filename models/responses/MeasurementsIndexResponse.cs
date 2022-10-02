using System;
using System.Globalization;
using System.Text.Json.Serialization;
using HazeMonitoring.models.dynamodb;

namespace HazeMonitoring.models.responses;

public class MeasurementsIndexResponse
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("reading")]
    public decimal Reading { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }

    public MeasurementsIndexResponse(MeasurementDbModel measurementDbModel)
    {
        Id = measurementDbModel.HashKey.Substring(measurementDbModel.HashKey.IndexOf('-') + 1);
        Reading = measurementDbModel.Reading;
        var timestampSuccess = DateTime.TryParse(measurementDbModel.SortKey.Split("#")[1], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var timestamp);
        Timestamp = timestampSuccess ? timestamp : default;
        Type = measurementDbModel.SortKey.Split("#")[0].Split("-")[1];
    }
}