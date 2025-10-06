using admin_application.Commands;
using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Interfaces;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Organizations;

public sealed class DeleteOrganizationCommandHandler(IOrganizationRepository repository) : IDeleteOrganizationCommandHandler
{
	public async Task<Result> HandleAsync(DeleteOrganizationCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<DeleteOrganizationCommandHandler>()
			.ForContext("Id", command.Id);

		log.Information("DeleteOrganization started");

		var result = await repository.DeleteAsync(command.Id, cancellationToken);

		log.Information("DeleteOrganization completed: {Success}", result.IsSuccess);

		return result;
	}
}