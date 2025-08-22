using admin_application.Commands;
using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Organizations;

public sealed class CreateOrganizationCommandHandler : ICreateOrganizationCommandHandler
{
    private readonly IOrganizationRepository _repository;
    public CreateOrganizationCommandHandler(IOrganizationRepository repository) { _repository = repository; }

    public async Task<Result<Organization>> HandleAsync(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<CreateOrganizationCommandHandler>()
            .ForContext("Name", command.Name);
        log.Information("CreateOrganization started");
        var model = new Organization { Id = Guid.NewGuid(), Name = command.Name };
        var result = await _repository.CreateAsync(model, cancellationToken);
        log.Information("CreateOrganization completed: {Success}", result.IsSuccess);
        return result;
    }
}


