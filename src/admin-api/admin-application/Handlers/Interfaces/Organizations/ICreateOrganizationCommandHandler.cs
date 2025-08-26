using admin_application.Commands;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Organizations;

public interface ICreateOrganizationCommandHandler
{
    Task<Result<Organization>> HandleAsync(CreateOrganizationCommand command, CancellationToken cancellationToken);
}