using admin_api.DTOs.Request;
using admin_api.DTOs.Response;
using admin_application.Commands;
using admin_application.Handlers.Interfaces.ApiKeys;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Queries;
using admin_domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/features")]
public sealed class FeaturesController : ControllerBase
{
    private const string ApiKeyHeader = "x-api-key";

    private readonly ICreateFeatureCommandHandler _createHandler;
    private readonly IUpdateFeatureCommandHandler _updateHandler;
    private readonly IDeleteFeatureCommandHandler _deleteHandler;
    private readonly IGetFeatureByIdQueryHandler _getByIdHandler;
    private readonly IListFeaturesQueryHandler _listHandler;
    private readonly IValidateApiKeyQueryHandler _apiKeyHandler;

    public FeaturesController(
        ICreateFeatureCommandHandler createHandler,
        IUpdateFeatureCommandHandler updateHandler,
        IDeleteFeatureCommandHandler deleteHandler,
        IGetFeatureByIdQueryHandler getByIdHandler,
        IListFeaturesQueryHandler listHandler,
        IValidateApiKeyQueryHandler apiKeyHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _listHandler = listHandler;
        _apiKeyHandler = apiKeyHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<FeatureResponse>>> List([FromQuery] Guid? projectId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("projectId", projectId);

        if (!await AuthorizeAsync(cancellationToken))
        {
            return Unauthorized();
        }

        log.Information("List features started");
        var result = await _listHandler.HandleAsync(new ListFeaturesQuery { ProjectId = projectId }, cancellationToken);

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

        if (!await AuthorizeAsync(cancellationToken))
        {
            return Unauthorized();
        }

        log.Information("Get feature started");

        var result = await _getByIdHandler.HandleAsync(new GetFeatureByIdQuery { Id = id }, cancellationToken);

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
    public async Task<ActionResult<FeatureResponse>> Create([FromBody] CreateFeatureRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeaturesController>()
            .ForContext("projectId", request.ProjectId)
            .ForContext("name", request.Name);

        if (!await AuthorizeAsync(cancellationToken))
        {
            return Unauthorized();
        }

        log.Information("Create feature started");

        var result = await _createHandler.HandleAsync(new CreateFeatureCommand
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

        if (!await AuthorizeAsync(cancellationToken))
        {
            return Unauthorized();
        }

        log.Information("Update feature started");

        var result = await _updateHandler.HandleAsync(new UpdateFeatureCommand
        {
            Id = id,
            ProjectId = request.ProjectId,
            Name = request.Name,
            Description = request.Description
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
        var log = Log.ForContext<FeaturesController>()
            .ForContext("id", id);

        if (!await AuthorizeAsync(cancellationToken))
        {
            return Unauthorized();
        }

        log.Information("Delete feature started");

        var result = await _deleteHandler.HandleAsync(new DeleteFeatureCommand { Id = id }, cancellationToken);

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

    private static FeatureResponse Map(Feature model) => new()
    {
        Id = model.Id,
        ProjectId = model.ProjectId,
        Name = model.Name,
        Description = model.Description
    };

    private async Task<bool> AuthorizeAsync(CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
        {
            Log.ForContext<FeaturesController>().Warning("Missing API key header");
            return false;
        }

        var valid = await _apiKeyHandler.HandleAsync(new ValidateApiKeyQuery { ApiKey = apiKey! }, cancellationToken);
        if (!valid)
        {
            Log.ForContext<FeaturesController>().Warning("API key invalid");
        }

        return valid;
    }
}


