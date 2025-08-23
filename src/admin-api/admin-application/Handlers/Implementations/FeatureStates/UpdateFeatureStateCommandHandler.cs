using admin_application.Commands;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class UpdateFeatureStateCommandHandler(IFeatureStateRepository repository) : IUpdateFeatureStateCommandHandler
{
	private readonly IFeatureStateRepository _repository = repository;

	public async Task<Result<FeatureState>> HandleAsync(UpdateFeatureStateCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<UpdateFeatureStateCommandHandler>()
			.ForContext("Id", command.Id)
			.ForContext("FeatureId", command.FeatureId)
			.ForContext("EnvironmentId", command.EnvironmentId)
			.ForContext("Enabled", command.Enabled);
		log.Information("UpdateFeatureState started");

		var model = new FeatureState { Id = command.Id, FeatureId = command.FeatureId, EnvironmentId = command.EnvironmentId, Enabled = command.Enabled, Reason = command.Reason };

		var result = await _repository.UpdateAsync(model, cancellationToken);

		log.Information("UpdateFeatureState completed: {Success}", result.IsSuccess);

		return result;
	}
}