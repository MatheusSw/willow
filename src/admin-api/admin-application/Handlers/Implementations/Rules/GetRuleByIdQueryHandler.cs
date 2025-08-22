using admin_application.Handlers.Interfaces.Rules;
using admin_application.Interfaces;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Rules;

public sealed class GetRuleByIdQueryHandler : IGetRuleByIdQueryHandler
{
    private readonly IRuleRepository _repository;

    public GetRuleByIdQueryHandler(IRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Rule>> HandleAsync(GetRuleByIdQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<GetRuleByIdQueryHandler>()
            .ForContext("Id", query.Id);
        log.Information("GetRuleById started");
        var result = await _repository.GetByIdAsync(query.Id, cancellationToken);
        log.Information("GetRuleById completed: {Success}", result.IsSuccess);
        return result;
    }
}


