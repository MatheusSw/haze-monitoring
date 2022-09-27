using System;
using System.Globalization;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace HazeMonitoring.models.dynamodb;

public class MeasurementDbModel
{
    [DynamoDBHashKey("PK")] [JsonIgnore] public string HashKey { get; set; }

    [DynamoDBRangeKey("SK")] [JsonIgnore] public string SortKey { get; set; }

    [DynamoDBProperty("Measurement-reading")]
    [JsonPropertyName("reading")]
    public decimal Reading { get; set; }

    [DynamoDBIgnore]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp
    {
        get
        {
            var success = DateTime.TryParse(SortKey.Split("#")[1], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var timestamp);
            return success ? timestamp : default;
        }
    }
    
    [DynamoDBIgnore]
    [JsonPropertyName("type")]
    public string Type => SortKey.Split("#")[0].Split("-")[1];
}