using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class GetFeatureStateByIdQueryHandler : IGetFeatureStateByIdQueryHandler
{
    private readonly IFeatureStateRepository _repository;
    public GetFeatureStateByIdQueryHandler(IFeatureStateRepository repository) { _repository = repository; }

    public async Task<Result<FeatureState>> HandleAsync(GetFeatureStateByIdQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<GetFeatureStateByIdQueryHandler>()
            .ForContext("Id", query.Id);
        log.Information("GetFeatureStateById started");
        var result = await _repository.GetByIdAsync(query.Id, cancellationToken);
        log.Information("GetFeatureStateById completed: {Success}", result.IsSuccess);
        return result;
    }
}


