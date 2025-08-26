using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Features;

public interface IListFeaturesQueryHandler
{
    Task<Result<List<Feature>>> HandleAsync(ListFeaturesQuery query, CancellationToken cancellationToken);
}