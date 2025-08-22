using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Interfaces;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Organizations;

public sealed class GetOrganizationByIdQueryHandler : IGetOrganizationByIdQueryHandler
{
    private readonly IOrganizationRepository _repository;
    public GetOrganizationByIdQueryHandler(IOrganizationRepository repository) { _repository = repository; }

    public async Task<Result<Organization>> HandleAsync(GetOrganizationByIdQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<GetOrganizationByIdQueryHandler>()
            .ForContext("Id", query.Id);
        log.Information("GetOrganizationById started");
        var result = await _repository.GetByIdAsync(query.Id, cancellationToken);
        log.Information("GetOrganizationById completed: {Success}", result.IsSuccess);
        return result;
    }
}


