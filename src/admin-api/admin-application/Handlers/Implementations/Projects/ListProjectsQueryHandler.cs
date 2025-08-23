using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain;
using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class ListProjectsQueryHandler(IProjectRepository repository) : IListProjectsQueryHandler
{
	private readonly IProjectRepository _repository = repository;

	public async Task<Result<List<Project>>> HandleAsync(ListProjectsQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ListProjectsQueryHandler>()
			.ForContext("OrgId", query.OrgId);
		log.Information("ListProjects started");

		var result = await _repository.ListAsync(query.OrgId, cancellationToken);

		log.Information("ListProjects completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);

		return result;
	}
}