using admin_application.Commands;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Interfaces;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Features;

public sealed class DeleteFeatureCommandHandler(IFeatureRepository repository) : IDeleteFeatureCommandHandler
{
	private readonly IFeatureRepository _repository = repository;

	public async Task<Result> HandleAsync(DeleteFeatureCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<DeleteFeatureCommandHandler>()
			.ForContext("Id", command.Id);
		log.Information("DeleteFeature started");

		var result = await _repository.DeleteAsync(command.Id, cancellationToken);

		log.Information("DeleteFeature completed: {Success}", result.IsSuccess);

		return result;
	}
}