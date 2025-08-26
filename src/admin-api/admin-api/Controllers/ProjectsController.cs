using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Commands;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain;
using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace admin_api.Controllers;

[ApiController]
[Route("v1/projects")]
public sealed class ProjectsController(
	ICreateProjectCommandHandler createHandler,
	IUpdateProjectCommandHandler updateHandler,
	IDeleteProjectCommandHandler deleteHandler,
	IGetProjectByIdQueryHandler getByIdHandler,
	IListProjectsQueryHandler listHandler) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<ProjectResponse>>> List([FromQuery] Guid? orgId, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectsController>()
			.ForContext("orgId", orgId);

		log.Information("List projects started");
		var result = await listHandler.HandleAsync(new ListProjectsQuery { OrgId = orgId }, cancellationToken);

		if (result.IsFailed)
		{
			return Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		var response = result.Value.Select(Map).ToList();
		if (response.Count == 0)
		{
			log.Information("List projects completed: no content");
			return NoContent();
		}

		log.Information("List projects completed: {Count}", response.Count);
		return Ok(response);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<ProjectResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectsController>()
			.ForContext("id", id);

		log.Information("Get project started");

		var result = await getByIdHandler.HandleAsync(new GetProjectByIdQuery { Id = id }, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? (ActionResult<ProjectResponse>)NotFound()
				: (ActionResult<ProjectResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return Ok(Map(result.Value));
	}

	[HttpPost]
	public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectsController>()
			.ForContext("orgId", request.OrgId)
			.ForContext("name", request.Name);

		log.Information("Create project started");

		var result = await createHandler.HandleAsync(new CreateProjectCommand
		{
			OrgId = request.OrgId,
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
	public async Task<ActionResult<ProjectResponse>> Update([FromRoute] Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectsController>()
			.ForContext("id", id)
			.ForContext("orgId", request.OrgId)
			.ForContext("name", request.Name);

		log.Information("Update project started");

		var result = await updateHandler.HandleAsync(new UpdateProjectCommand
		{
			Id = id,
			OrgId = request.OrgId,
			Name = request.Name
		}, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? (ActionResult<ProjectResponse>)NotFound()
				: (ActionResult<ProjectResponse>)Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return Ok(Map(result.Value));
	}

	[HttpDelete("{id:guid}")]
	public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectsController>()
			.ForContext("id", id);

		log.Information("Delete project started");

		var result = await deleteHandler.HandleAsync(new DeleteProjectCommand { Id = id }, cancellationToken);

		if (result.IsFailed)
		{
			return result.Errors.Any(e => e.Message == "NotFound")
				? NotFound()
				: Problem(statusCode: 500, detail: string.Join(";", result.Errors.Select(e => e.Message)));
		}

		return NoContent();
	}

	private static ProjectResponse Map(Project model) => new()
	{
		Id = model.Id,
		OrgId = model.OrgId,
		Name = model.Name
	};
}