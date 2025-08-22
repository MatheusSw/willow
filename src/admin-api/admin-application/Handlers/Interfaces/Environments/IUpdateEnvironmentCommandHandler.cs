using admin_application.Commands;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Environments;

public interface IUpdateEnvironmentCommandHandler
{
    Task<Result<admin_domain.Entities.Environment>> HandleAsync(UpdateEnvironmentCommand command, CancellationToken cancellationToken);
}


