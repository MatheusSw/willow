using admin_api.DTOs.Request;
using admin_api.DTOs.Response;
using admin_application.Commands;
using admin_application.Handlers.Interfaces.Rules;
using admin_application.Queries;
using admin_domain.Entities;
using admin_domain.Rules;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/rules")]
public sealed class RulesController : ControllerBase
{
    private readonly ICreateRuleCommandHandler _createHandler;
    private readonly IUpdateRuleCommandHandler _updateHandler;
    private readonly IDeleteRuleCommandHandler _deleteHandler;
    private readonly IGetRuleByIdQueryHandler _getByIdHandler;
    private readonly IListRulesQueryHandler _listHandler;

    public RulesController(
        ICreateRuleCommandHandler createHandler,
        IUpdateRuleCommandHandler updateHandler,
        IDeleteRuleCommandHandler deleteHandler,
        IGetRuleByIdQueryHandler getByIdHandler,
        IListRulesQueryHandler listHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _listHandler = listHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<RuleResponse>>> List([FromQuery] Guid? featureId, [FromQuery] Guid? environmentId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RulesController>()
            .ForContext("featureId", featureId)
            .ForContext("environmentId", environmentId);

        log.Information("List rules started");
        var result = await _listHandler.HandleAsync(new ListRulesQuery { FeatureId = featureId, EnvironmentId = environmentId }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var response = result.Value.Select(Map).ToList();
        if (response.Count == 0)
        {
            log.Information("List rules completed: no content");
            return NoContent();
        }

        log.Information("List rules completed: {Count}", response.Count);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RuleResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RulesController>()
            .ForContext("id", id);

        log.Information("Get rule started");

        var result = await _getByIdHandler.HandleAsync(new GetRuleByIdQuery { Id = id }, cancellationToken);

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
    public async Task<ActionResult<RuleResponse>> Create([FromBody] CreateRuleRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RulesController>()
            .ForContext("featureId", request.FeatureId)
            .ForContext("environmentId", request.EnvironmentId)
            .ForContext("priority", request.Priority)
            .ForContext("matchType", request.MatchType);

        log.Information("Create rule started");

        var result = await _createHandler.HandleAsync(new CreateRuleCommand
        {
            FeatureId = request.FeatureId,
            EnvironmentId = request.EnvironmentId,
            Priority = request.Priority,
            MatchType = string.Equals(request.MatchType, "any", StringComparison.OrdinalIgnoreCase) ? "any" : "all",
            Conditions = request.Conditions.Select(c => new RuleCondition
            {
                Attribute = c.Attribute,
                Op = c.Op,
                Value = c.Value
            }).ToList()
        }, cancellationToken);

        if (result.IsFailed)
        {
            return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
        }

        var created = Map(result.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RuleResponse>> Update([FromRoute] Guid id, [FromBody] UpdateRuleRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RulesController>()
            .ForContext("id", id)
            .ForContext("featureId", request.FeatureId)
            .ForContext("environmentId", request.EnvironmentId)
            .ForContext("priority", request.Priority)
            .ForContext("matchType", request.MatchType);

        log.Information("Update rule started");

        var result = await _updateHandler.HandleAsync(new UpdateRuleCommand
        {
            Id = id,
            FeatureId = request.FeatureId,
            EnvironmentId = request.EnvironmentId,
            Priority = request.Priority,
            MatchType = string.Equals(request.MatchType, "any", StringComparison.OrdinalIgnoreCase) ? "any" : "all",
            Conditions = request.Conditions.Select(c => new RuleCondition
            {
                Attribute = c.Attribute,
                Op = c.Op,
                Value = c.Value
            }).ToList()
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
        var log = Log.ForContext<RulesController>()
            .ForContext("id", id);

        log.Information("Delete rule started");

        var result = await _deleteHandler.HandleAsync(new DeleteRuleCommand { Id = id }, cancellationToken);

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

    private static RuleResponse Map(Rule model) => new()
    {
        Id = model.Id,
        FeatureId = model.FeatureId,
        EnvironmentId = model.EnvironmentId,
        Priority = model.Priority,
        MatchType = model.MatchType == admin_domain.Rules.MatchType.Any ? "any" : "all",
        Conditions = model.Conditions.Select(c => new RuleConditionResponse
        {
            Attribute = c.Attribute,
            Op = c.Op,
            Value = c.Value
        }).ToList()
    };
}


