using System;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace HazeMonitoring.models.dynamodb;

public class MeasurementDbModel
{
    [DynamoDBHashKey("PK")]
    [JsonPropertyName("id")]
    public string HashKey { get; set; }

    [DynamoDBRangeKey("SK")] [JsonIgnore] public string SortKey { get; set; }

    [DynamoDBProperty("Measurement-reading")]
    [JsonPropertyName("reading")]
    public decimal Reading { get; set; }
    
    [DynamoDBIgnore]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}