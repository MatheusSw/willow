using admin_application.Commands;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Environments;

public interface IDeleteEnvironmentCommandHandler
{
    Task<Result> HandleAsync(DeleteEnvironmentCommand command, CancellationToken cancellationToken);
}


