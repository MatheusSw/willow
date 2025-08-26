using admin_application.Queries;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Environments;

public interface IGetEnvironmentByIdQueryHandler
{
    Task<Result<admin_domain.Entities.Environment>> HandleAsync(GetEnvironmentByIdQuery query, CancellationToken cancellationToken);
}