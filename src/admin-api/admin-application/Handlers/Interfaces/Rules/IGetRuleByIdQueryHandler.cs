using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Rules;

public interface IGetRuleByIdQueryHandler
{
    Task<Result<Rule>> HandleAsync(GetRuleByIdQuery query, CancellationToken cancellationToken);
}


