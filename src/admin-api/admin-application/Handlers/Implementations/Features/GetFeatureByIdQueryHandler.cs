using admin_application.Handlers.Interfaces.Features;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Features;

public sealed class GetFeatureByIdQueryHandler(IFeatureRepository repository) : IGetFeatureByIdQueryHandler
{
	public async Task<Result<Feature>> HandleAsync(GetFeatureByIdQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<GetFeatureByIdQueryHandler>()
			.ForContext("Id", query.Id);
		log.Information("GetFeatureById started");

		var result = await repository.GetByIdAsync(query.Id, cancellationToken);

		log.Information("GetFeatureById completed: {Success}", result.IsSuccess);

		return result;
	}
}