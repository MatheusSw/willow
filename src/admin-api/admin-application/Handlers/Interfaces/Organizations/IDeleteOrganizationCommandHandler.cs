using admin_application.Commands;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Organizations;

public interface IDeleteOrganizationCommandHandler
{
    Task<Result> HandleAsync(DeleteOrganizationCommand command, CancellationToken cancellationToken);
}


