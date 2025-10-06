using admin_application.Commands;
using admin_application.Handlers.Interfaces.Environments;
using admin_application.Interfaces;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Environments;

public sealed class UpdateEnvironmentCommandHandler(IEnvironmentRepository repository) : IUpdateEnvironmentCommandHandler
{
	public async Task<Result<admin_domain.Entities.Environment>> HandleAsync(UpdateEnvironmentCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<UpdateEnvironmentCommandHandler>()
			.ForContext("Id", command.Id)
			.ForContext("ProjectId", command.ProjectId)
			.ForContext("Key", command.Key);
		log.Information("UpdateEnvironment started");

		var model = new admin_domain.Entities.Environment { Id = command.Id, ProjectId = command.ProjectId, Key = command.Key };

		var result = await repository.UpdateAsync(model, cancellationToken);

		log.Information("UpdateEnvironment completed: {Success}", result.IsSuccess);

		return result;
	}
}