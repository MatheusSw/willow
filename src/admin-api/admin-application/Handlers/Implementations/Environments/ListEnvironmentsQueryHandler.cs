using admin_application.Handlers.Interfaces.Environments;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Environments;

public sealed class ListEnvironmentsQueryHandler(IEnvironmentRepository repository) : IListEnvironmentsQueryHandler
{
	public async Task<Result<List<admin_domain.Entities.Environment>>> HandleAsync(ListEnvironmentsQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ListEnvironmentsQueryHandler>()
			.ForContext("ProjectId", query.ProjectId);
		log.Information("ListEnvironments started");

		var result = await repository.ListAsync(query.ProjectId, cancellationToken);

		log.Information("ListEnvironments completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);

		return result;
	}
}