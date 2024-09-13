using Haze.Application.Commands.Clusters;
using Haze.Application.UseCases.Clusters;
using Haze.Infra.Models.Clusters;
using HazeApi.Models.Cluster;
using Microsoft.AspNetCore.Mvc;

namespace HazeApi.Controllers;

[ApiController]
[Route("clusters")]
public class ClusterController(IClusterCommandHandler clusterCommandHandler) : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Empty;
    }

    [HttpPost]
    public ActionResult<Cluster?> Store(ClusterStoreRequest request)
    {
        var command = new CreateClusterCommand
        {
            Name = request.Name,
            Location = request.Location,
        };

        var result = clusterCommandHandler.Handle(command);

        return result;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Cluster?>> Update(ClusterUpdateRequest request, string id)
    {
        var command = new UpdateClusterCommand
        {
            Id = id,
            Name = request.Name,
            Location = request.Location,
        };

        var result = await clusterCommandHandler.Handle(command);

        return result;
    }
}