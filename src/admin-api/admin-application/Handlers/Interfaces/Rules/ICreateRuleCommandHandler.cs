using admin_application.Commands;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Rules;

public interface ICreateRuleCommandHandler
{
    Task<Result<Rule>> HandleAsync(CreateRuleCommand command, CancellationToken cancellationToken);
}


