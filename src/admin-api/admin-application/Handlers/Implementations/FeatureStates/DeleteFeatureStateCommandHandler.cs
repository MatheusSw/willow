using admin_application.Commands;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class DeleteFeatureStateCommandHandler : IDeleteFeatureStateCommandHandler
{
    private readonly IFeatureStateRepository _repository;
    public DeleteFeatureStateCommandHandler(IFeatureStateRepository repository) { _repository = repository; }

    public async Task<Result> HandleAsync(DeleteFeatureStateCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<DeleteFeatureStateCommandHandler>()
            .ForContext("Id", command.Id);
        log.Information("DeleteFeatureState started");
        var result = await _repository.DeleteAsync(command.Id, cancellationToken);
        log.Information("DeleteFeatureState completed: {Success}", result.IsSuccess);
        return result;
    }
}


