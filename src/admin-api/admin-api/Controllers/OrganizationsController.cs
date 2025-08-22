using admin_api.DTOs.Request;
using admin_api.DTOs.Response;
using admin_application.Commands;
using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Queries;
using admin_domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly ICreateOrganizationCommandHandler _createHandler;
    private readonly IUpdateOrganizationCommandHandler _updateHandler;
    private readonly IDeleteOrganizationCommandHandler _deleteHandler;
    private readonly IGetOrganizationByIdQueryHandler _getByIdHandler;
    private readonly IListOrganizationsQueryHandler _listHandler;

    public OrganizationsController(
        ICreateOrganizationCommandHandler createHandler,
        IUpdateOrganizationCommandHandler updateHandler,
        IDeleteOrganizationCommandHandler deleteHandler,
        IGetOrganizationByIdQueryHandler getByIdHandler,
        IListOrganizationsQueryHandler listHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _listHandler = listHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrganizationResponse>>> List([FromQuery] string? name, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<OrganizationsController>()
            .ForContext("name", name);

        log.Information("List organizations started");
        var result = await _listHandler.HandleAsync(new ListOrganizationsQuery { Name = name }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var response = result.Value.Select(Map).ToList();
        if (response.Count == 0)
        {
            log.Information("List organizations completed: no content");
            return NoContent();
        }

        log.Information("List organizations completed: {Count}", response.Count);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<OrganizationsController>()
            .ForContext("id", id);

        log.Information("Get organization started");

        var result = await _getByIdHandler.HandleAsync(new GetOrganizationByIdQuery { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "NotFound"))
            {
                return NotFound();
            }

            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return Ok(Map(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<OrganizationsController>()
            .ForContext("name", request.Name);

        log.Information("Create organization started");

        var result = await _createHandler.HandleAsync(new CreateOrganizationCommand
        {
            Name = request.Name
        }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var created = Map(result.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> Update([FromRoute] Guid id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<OrganizationsController>()
            .ForContext("id", id)
            .ForContext("name", request.Name);

        log.Information("Update organization started");

        var result = await _updateHandler.HandleAsync(new UpdateOrganizationCommand
        {
            Id = id,
            Name = request.Name
        }, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "NotFound"))
            {
                return NotFound();
            }

            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return Ok(Map(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<OrganizationsController>()
            .ForContext("id", id);

        log.Information("Delete organization started");

        var result = await _deleteHandler.HandleAsync(new DeleteOrganizationCommand { Id = id }, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "NotFound"))
            {
                return NotFound();
            }

            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        return NoContent();
    }

    private static OrganizationResponse Map(Organization model) => new()
    {
        Id = model.Id,
        Name = model.Name
    };
}


