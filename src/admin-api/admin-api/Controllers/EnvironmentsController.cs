using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Commands;
using admin_application.Handlers.Interfaces.Environments;
using admin_application.Queries;

using admin_domain.Entities;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/environments")]
public sealed class EnvironmentsController(
    ICreateEnvironmentCommandHandler createHandler,
    IUpdateEnvironmentCommandHandler updateHandler,
    IDeleteEnvironmentCommandHandler deleteHandler,
    IGetEnvironmentByIdQueryHandler getByIdHandler,
    IListEnvironmentsQueryHandler listHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EnvironmentResponse>>> List([FromQuery] Guid? projectId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentsController>()
            .ForContext("projectId", projectId);

        log.Information("List environments started");
        var result = await listHandler.HandleAsync(new ListEnvironmentsQuery { ProjectId = projectId }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var response = result.Value.Select(Map).ToList();
        if (response.Count == 0)
        {
            log.Information("List environments completed: no content");
            return NoContent();
        }

        log.Information("List environments completed: {Count}", response.Count);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EnvironmentResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentsController>()
            .ForContext("id", id);

        log.Information("Get environment started");

        var result = await getByIdHandler.HandleAsync(new GetEnvironmentByIdQuery { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.Any(e => e.Message == "NotFound")
                ? (ActionResult<EnvironmentResponse>)NotFound()
                : (ActionResult<EnvironmentResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return Ok(Map(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentResponse>> Create([FromBody] CreateEnvironmentRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentsController>()
            .ForContext("projectId", request.ProjectId)
            .ForContext("key", request.Key);

        log.Information("Create environment started");

        var result = await createHandler.HandleAsync(new CreateEnvironmentCommand
        {
            ProjectId = request.ProjectId,
            Key = request.Key
        }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var created = Map(result.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EnvironmentResponse>> Update([FromRoute] Guid id, [FromBody] UpdateEnvironmentRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentsController>()
            .ForContext("id", id)
            .ForContext("projectId", request.ProjectId)
            .ForContext("key", request.Key);

        log.Information("Update environment started");

        var result = await updateHandler.HandleAsync(new UpdateEnvironmentCommand
        {
            Id = id,
            ProjectId = request.ProjectId,
            Key = request.Key
        }, cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.Any(e => e.Message == "NotFound")
                ? (ActionResult<EnvironmentResponse>)NotFound()
                : (ActionResult<EnvironmentResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return Ok(Map(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentsController>()
            .ForContext("id", id);

        log.Information("Delete environment started");

        var result = await deleteHandler.HandleAsync(new DeleteEnvironmentCommand { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.Any(e => e.Message == "NotFound")
                ? NotFound()
                : Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return NoContent();
    }

    private static EnvironmentResponse Map(admin_domain.Entities.Environment model) => new()
    {
        Id = model.Id,
        ProjectId = model.ProjectId,
        Key = model.Key
    };
}