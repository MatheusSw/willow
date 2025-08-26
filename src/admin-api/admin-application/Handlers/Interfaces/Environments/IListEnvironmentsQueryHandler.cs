using admin_application.Queries;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Environments;

public interface IListEnvironmentsQueryHandler
{
    Task<Result<List<admin_domain.Entities.Environment>>> HandleAsync(ListEnvironmentsQuery query, CancellationToken cancellationToken);
}