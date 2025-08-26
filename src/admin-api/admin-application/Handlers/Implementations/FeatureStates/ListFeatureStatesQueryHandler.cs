using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class ListFeatureStatesQueryHandler(IFeatureStateRepository repository) : IListFeatureStatesQueryHandler
{
    public async Task<Result<List<FeatureState>>> HandleAsync(ListFeatureStatesQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<ListFeatureStatesQueryHandler>()
            .ForContext("FeatureId", query.FeatureId)
            .ForContext("EnvironmentId", query.EnvironmentId);
        log.Information("ListFeatureStates started");

        var result = await repository.ListAsync(query.FeatureId, query.EnvironmentId, cancellationToken);

        log.Information("ListFeatureStates completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);

        return result;
    }
}