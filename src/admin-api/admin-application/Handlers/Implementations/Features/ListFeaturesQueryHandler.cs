using admin_application.Handlers.Interfaces.Features;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Features;

public sealed class ListFeaturesQueryHandler(IFeatureRepository repository) : IListFeaturesQueryHandler
{
	private readonly IFeatureRepository _repository = repository;

	public async Task<Result<List<Feature>>> HandleAsync(ListFeaturesQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ListFeaturesQueryHandler>()
			.ForContext("ProjectId", query.ProjectId);
		log.Information("ListFeatures started");
		var result = await _repository.ListAsync(query.ProjectId, cancellationToken);
		log.Information("ListFeatures completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);
		return result;
	}
}