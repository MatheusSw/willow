using admin_application.Commands;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Interfaces;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Features;

public sealed class CreateFeatureCommandHandler(IFeatureRepository repository) : ICreateFeatureCommandHandler
{
	private readonly IFeatureRepository _repository = repository;

	public async Task<Result<Feature>> HandleAsync(CreateFeatureCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<CreateFeatureCommandHandler>()
			.ForContext("ProjectId", command.ProjectId)
			.ForContext("Name", command.Name);

		log.Information("CreateFeature started");

		var model = new Feature { Id = Guid.NewGuid(), ProjectId = command.ProjectId, Name = command.Name, Description = command.Description };

		var result = await _repository.CreateAsync(model, cancellationToken);

		log.Information("CreateFeature completed: {Success}", result.IsSuccess);

		return result;
	}
}