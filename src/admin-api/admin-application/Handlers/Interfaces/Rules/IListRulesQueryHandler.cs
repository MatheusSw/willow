using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Rules;

public interface IListRulesQueryHandler
{
	Task<Result<List<Rule>>> HandleAsync(ListRulesQuery query, CancellationToken cancellationToken);
}