using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.FeatureStates;

public interface IGetFeatureStateByIdQueryHandler
{
    Task<Result<FeatureState>> HandleAsync(GetFeatureStateByIdQuery query, CancellationToken cancellationToken);
}