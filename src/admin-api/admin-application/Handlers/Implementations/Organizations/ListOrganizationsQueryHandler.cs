using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Organizations;

public sealed class ListOrganizationsQueryHandler(IOrganizationRepository repository) : IListOrganizationsQueryHandler
{
    public async Task<Result<List<Organization>>> HandleAsync(ListOrganizationsQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<ListOrganizationsQueryHandler>()
            .ForContext("Name", query.Name);
        log.Information("ListOrganizations started");

        var result = await repository.ListAsync(query.Name, cancellationToken);

        log.Information("ListOrganizations completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);

        return result;
    }
}