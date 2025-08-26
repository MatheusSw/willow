using admin_application.Handlers.Interfaces.Environments;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Environments;

public sealed class GetEnvironmentByIdQueryHandler(IEnvironmentRepository repository) : IGetEnvironmentByIdQueryHandler
{
	private readonly IEnvironmentRepository _repository = repository;

	public async Task<Result<admin_domain.Entities.Environment>> HandleAsync(GetEnvironmentByIdQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<GetEnvironmentByIdQueryHandler>()
			.ForContext("Id", query.Id);
		log.Information("GetEnvironmentById started");

		var result = await _repository.GetByIdAsync(query.Id, cancellationToken);

		log.Information("GetEnvironmentById completed: {Success}", result.IsSuccess);

		return result;
	}
}