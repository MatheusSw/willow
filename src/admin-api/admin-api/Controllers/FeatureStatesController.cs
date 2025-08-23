using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Commands;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Queries;

using admin_domain.Entities;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/feature-states")]
public sealed class FeatureStatesController(
	ICreateFeatureStateCommandHandler createHandler,
	IUpdateFeatureStateCommandHandler updateHandler,
	IDeleteFeatureStateCommandHandler deleteHandler,
	IGetFeatureStateByIdQueryHandler getByIdHandler,
	IListFeatureStatesQueryHandler listHandler) : ControllerBase
{
	private readonly ICreateFeatureStateCommandHandler _createHandler = createHandler;
	private readonly IUpdateFeatureStateCommandHandler _updateHandler = updateHandler;
	private readonly IDeleteFeatureStateCommandHandler _deleteHandler = deleteHandler;
	private readonly IGetFeatureStateByIdQueryHandler _getByIdHandler = getByIdHandler;
	private readonly IListFeatureStatesQueryHandler _listHandler = listHandler;

	[HttpGet]
	public async Task<ActionResult<List<FeatureStateResponse>>> List([FromQuery] Guid? featureId, [FromQuery] Guid? environmentId, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureStatesController>()
			.ForContext("featureId", featureId)
			.ForContext("environmentId", environmentId);

		log.Information("List feature states started");
		var result = await _listHandler.HandleAsync(new ListFeatureStatesQuery { FeatureId = featureId, EnvironmentId = environmentId }, cancellationToken);

		if (result.IsFailed)
		{
			return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		var response = result.Value.Select(Map).ToList();
		if (response.Count == 0)
		{
			log.Information("List feature states completed: no content");
			return NoContent();
		}

		log.Information("List feature states completed: {Count}", response.Count);
		return Ok(response);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<FeatureStateResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureStatesController>()
			.ForContext("id", id);

		log.Information("Get feature state started");

		var result = await _getByIdHandler.HandleAsync(new GetFeatureStateByIdQuery { Id = id }, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? (ActionResult<FeatureStateResponse>)NotFound()
				: (ActionResult<FeatureStateResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return Ok(Map(result.Value));
	}

	[HttpPost]
	public async Task<ActionResult<FeatureStateResponse>> Create([FromBody] CreateFeatureStateRequest request, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureStatesController>()
			.ForContext("featureId", request.FeatureId)
			.ForContext("environmentId", request.EnvironmentId)
			.ForContext("enabled", request.Enabled);

		log.Information("Create feature state started");

		var result = await _createHandler.HandleAsync(new CreateFeatureStateCommand
		{
			FeatureId = request.FeatureId,
			EnvironmentId = request.EnvironmentId,
			Enabled = request.Enabled,
			Reason = request.Reason
		}, cancellationToken);

		if (result.IsFailed)
		{
			return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		var created = Map(result.Value);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id:guid}")]
	public async Task<ActionResult<FeatureStateResponse>> Update([FromRoute] Guid id, [FromBody] UpdateFeatureStateRequest request, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureStatesController>()
			.ForContext("id", id)
			.ForContext("featureId", request.FeatureId)
			.ForContext("environmentId", request.EnvironmentId)
			.ForContext("enabled", request.Enabled);

		log.Information("Update feature state started");

		var result = await _updateHandler.HandleAsync(new UpdateFeatureStateCommand
		{
			Id = id,
			FeatureId = request.FeatureId,
			EnvironmentId = request.EnvironmentId,
			Enabled = request.Enabled,
			Reason = request.Reason
		}, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? (ActionResult<FeatureStateResponse>)NotFound()
				: (ActionResult<FeatureStateResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return Ok(Map(result.Value));
	}

	[HttpDelete("{id:guid}")]
	public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureStatesController>()
			.ForContext("id", id);

		log.Information("Delete feature state started");

		var result = await _deleteHandler.HandleAsync(new DeleteFeatureStateCommand { Id = id }, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? NotFound()
				: Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return NoContent();
	}

	private static FeatureStateResponse Map(FeatureState model) => new()
	{
		Id = model.Id,
		FeatureId = model.FeatureId,
		EnvironmentId = model.EnvironmentId,
		Enabled = model.Enabled,
		Reason = model.Reason
	};
}