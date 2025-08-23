using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain;
using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class GetProjectByIdQueryHandler(IProjectRepository repository) : IGetProjectByIdQueryHandler
{
	private readonly IProjectRepository _repository = repository;

	public async Task<Result<Project>> HandleAsync(GetProjectByIdQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<GetProjectByIdQueryHandler>()
			.ForContext("Id", query.Id);
		log.Information("GetProjectById started");

		var result = await _repository.GetByIdAsync(query.Id, cancellationToken);

		log.Information("GetProjectById completed: {Success}", result.IsSuccess);

		return result;
	}
}