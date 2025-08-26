using admin_application.Commands;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class CreateFeatureStateCommandHandler(IFeatureStateRepository repository) : ICreateFeatureStateCommandHandler
{
	private readonly IFeatureStateRepository _repository = repository;

	public async Task<Result<FeatureState>> HandleAsync(CreateFeatureStateCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<CreateFeatureStateCommandHandler>()
			.ForContext("FeatureId", command.FeatureId)
			.ForContext("EnvironmentId", command.EnvironmentId)
			.ForContext("Enabled", command.Enabled);
		log.Information("CreateFeatureState started");

		var model = new FeatureState { Id = Guid.NewGuid(), FeatureId = command.FeatureId, EnvironmentId = command.EnvironmentId, Enabled = command.Enabled, Reason = command.Reason };

		var result = await _repository.CreateAsync(model, cancellationToken);

		log.Information("CreateFeatureState completed: {Success}", result.IsSuccess);

		return result;
	}
}