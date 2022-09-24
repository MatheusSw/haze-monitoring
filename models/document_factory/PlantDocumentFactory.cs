using Amazon.DynamoDBv2.DocumentModel;

namespace HazeMonitoring.models.document_factory;

public static class PlantDocumentFactory
{
    public static Document Make(Plant plant)
    {
        return new Document
        {
            ["PK"] = ClusterDocumentFactory.GeneratePrimaryKeyFromClusterId(plant.ClusterId),
            ["SK"] = $"plant-{plant.Id}",
            ["Plant-strain"] = plant.Strain,
            ["Plant-lifetime"] = plant.Lifetime,
            ["Plant-state"] = plant.State,
            ["Plant-cloned-from"] = plant.ClonedFrom
        };
    }
}