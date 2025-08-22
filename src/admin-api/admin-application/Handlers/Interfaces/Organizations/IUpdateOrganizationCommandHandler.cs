using admin_application.Commands;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Organizations;

public interface IUpdateOrganizationCommandHandler
{
    Task<Result<Organization>> HandleAsync(UpdateOrganizationCommand command, CancellationToken cancellationToken);
}


