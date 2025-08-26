using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Commands;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Queries;

using admin_domain.Entities;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/features")]
public sealed class FeaturesController(
    ICreateFeatureCommandHandler createHandler,
    IUpdateFeatureCommandHandler updateHandler,
    IDeleteFeatureCommandHandler deleteHandler,
    IGetFeatureByIdQueryHandler getByIdHandler,
    IListFeaturesQueryHandler listHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FeatureResponse>>> List([FromQuery] Guid? projectId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("projectId", projectId);

        log.Information("List features started");
        var result = await listHandler.HandleAsync(new ListFeaturesQuery { ProjectId = projectId }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var response = result.Value.Select(Map).ToList();
        if (response.Count == 0)
        {
            log.Information("List features completed: no content");
            return NoContent();
        }

        log.Information("List features completed: {Count}", response.Count);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FeatureResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("id", id);

        log.Information("Get feature started");

        var result = await getByIdHandler.HandleAsync(new GetFeatureByIdQuery { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.Any(e => e.Message == "NotFound")
                ? (ActionResult<FeatureResponse>)NotFound()
                : (ActionResult<FeatureResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return Ok(Map(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<FeatureResponse>> Create([FromBody] CreateFeatureRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("projectId", request.ProjectId)
            .ForContext("name", request.Name);

        log.Information("Create feature started");

        var result = await createHandler.HandleAsync(new CreateFeatureCommand
        {
            ProjectId = request.ProjectId,
            Name = request.Name,
            Description = request.Description
        }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var created = Map(result.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FeatureResponse>> Update([FromRoute] Guid id, [FromBody] UpdateFeatureRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("id", id)
            .ForContext("projectId", request.ProjectId)
            .ForContext("name", request.Name);

        log.Information("Update feature started");

        var result = await updateHandler.HandleAsync(new UpdateFeatureCommand
        {
            Id = id,
            ProjectId = request.ProjectId,
            Name = request.Name,
            Description = request.Description
        }, cancellationToken);

        return result.IsFailed
            ? (ActionResult<FeatureResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)))
            : (ActionResult<FeatureResponse>)Ok(Map(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("id", id);

        log.Information("Delete feature started");

        var result = await deleteHandler.HandleAsync(new DeleteFeatureCommand { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.Any(e => e.Message == "NotFound")
                ? NotFound()
                : Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return NoContent();
    }

    private static FeatureResponse Map(Feature model) => new()
    {
        Id = model.Id,
        ProjectId = model.ProjectId,
        Name = model.Name,
        Description = model.Description
    };
}