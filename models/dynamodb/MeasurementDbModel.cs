using Amazon.DynamoDBv2.DataModel;

namespace HazeMonitoring.models.dynamodb;

public class MeasurementDbModel
{
    [DynamoDBHashKey("PK")] public string HashKey { get; set; }

    [DynamoDBRangeKey("SK")] public string SortKey { get; set; }

    [DynamoDBProperty("Measurement-reading")]
    public decimal Reading { get; set; }
}