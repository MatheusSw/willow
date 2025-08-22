using admin_application.Commands;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.FeatureStates;

public interface ICreateFeatureStateCommandHandler
{
    Task<Result<FeatureState>> HandleAsync(CreateFeatureStateCommand command, CancellationToken cancellationToken);
}


