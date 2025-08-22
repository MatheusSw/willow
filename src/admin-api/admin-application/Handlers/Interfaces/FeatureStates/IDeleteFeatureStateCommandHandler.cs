using admin_application.Commands;
using FluentResults;

namespace admin_application.Handlers.Interfaces.FeatureStates;

public interface IDeleteFeatureStateCommandHandler
{
    Task<Result> HandleAsync(DeleteFeatureStateCommand command, CancellationToken cancellationToken);
}


