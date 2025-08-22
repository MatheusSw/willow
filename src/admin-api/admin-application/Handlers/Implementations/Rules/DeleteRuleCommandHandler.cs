using admin_application.Commands;
using admin_application.Handlers.Interfaces.Rules;
using admin_application.Interfaces;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Rules;

public sealed class DeleteRuleCommandHandler : IDeleteRuleCommandHandler
{
    private readonly IRuleRepository _repository;

    public DeleteRuleCommandHandler(IRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(DeleteRuleCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<DeleteRuleCommandHandler>()
            .ForContext("Id", command.Id);

        log.Information("DeleteRule started");

        var result = await _repository.DeleteAsync(command.Id, cancellationToken);

        log.Information("DeleteRule completed: {Success}", result.IsSuccess);

        return result;
    }
}


