using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.FeatureStates;

public interface IListFeatureStatesQueryHandler
{
	Task<Result<List<FeatureState>>> HandleAsync(ListFeatureStatesQuery query, CancellationToken cancellationToken);
}