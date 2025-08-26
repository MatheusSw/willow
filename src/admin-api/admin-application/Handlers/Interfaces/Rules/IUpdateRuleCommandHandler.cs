using admin_application.Commands;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Rules;

public interface IUpdateRuleCommandHandler
{
    Task<Result<Rule>> HandleAsync(UpdateRuleCommand command, CancellationToken cancellationToken);
}