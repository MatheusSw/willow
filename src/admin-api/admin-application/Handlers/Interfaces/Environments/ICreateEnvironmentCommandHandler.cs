using admin_application.Commands;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Environments;

public interface ICreateEnvironmentCommandHandler
{
    Task<Result<admin_domain.Entities.Environment>> HandleAsync(CreateEnvironmentCommand command, CancellationToken cancellationToken);
}