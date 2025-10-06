using admin_application.Commands;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.FeatureStates;

public interface IUpdateFeatureStateCommandHandler
{
	Task<Result<FeatureState>> HandleAsync(UpdateFeatureStateCommand command, CancellationToken cancellationToken);
}