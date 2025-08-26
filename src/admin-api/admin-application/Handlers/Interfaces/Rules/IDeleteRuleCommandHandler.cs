using admin_application.Commands;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Rules;

public interface IDeleteRuleCommandHandler
{
    Task<Result> HandleAsync(DeleteRuleCommand command, CancellationToken cancellationToken);
}