using admin_application.Commands;
using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Interfaces;

using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Organizations;

public sealed class UpdateOrganizationCommandHandler(IOrganizationRepository repository) : IUpdateOrganizationCommandHandler
{
    public async Task<Result<Organization>> HandleAsync(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<UpdateOrganizationCommandHandler>()
            .ForContext("Id", command.Id)
            .ForContext("Name", command.Name);

        log.Information("UpdateOrganization started");

        var model = new Organization { Id = command.Id, Name = command.Name };

        var result = await repository.UpdateAsync(model, cancellationToken);

        log.Information("UpdateOrganization completed: {Success}", result.IsSuccess);

        return result;
    }
}