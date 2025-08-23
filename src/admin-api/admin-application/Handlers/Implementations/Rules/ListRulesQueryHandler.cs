using admin_application.Handlers.Interfaces.Rules;
using admin_application.Interfaces;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Rules;

public sealed class ListRulesQueryHandler(IRuleRepository repository) : IListRulesQueryHandler
{
	private readonly IRuleRepository _repository = repository;

	public async Task<Result<List<Rule>>> HandleAsync(ListRulesQuery query, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ListRulesQueryHandler>()
			.ForContext("FeatureId", query.FeatureId)
			.ForContext("EnvironmentId", query.EnvironmentId);
		log.Information("ListRules started");
		var result = await _repository.ListAsync(query.FeatureId, query.EnvironmentId, cancellationToken);
		log.Information("ListRules completed: {Success} Count={Count}", result.IsSuccess, result.ValueOrDefault?.Count ?? 0);
		return result;
	}
}