using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Organizations;

public interface IListOrganizationsQueryHandler
{
    Task<Result<List<Organization>>> HandleAsync(ListOrganizationsQuery query, CancellationToken cancellationToken);
}


