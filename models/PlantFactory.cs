using System;
using HazeMonitoring.models.requests;

namespace HazeMonitoring.models;

public static class PlantFactory
{
    public static Plant Make(string clusterId, PlantCreateRequest plantCreateRequest)
    {
        return new Plant
        {
            ClusterId = clusterId,
            Lifetime = plantCreateRequest.Lifetime,
            State = plantCreateRequest.State,
            Strain = plantCreateRequest.Strain,
            ClonedFrom = plantCreateRequest.ClonedFrom,
            Id = Guid.NewGuid().ToString()
        };
    }
}