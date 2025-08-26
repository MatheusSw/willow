using admin_application.Commands;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Interfaces;

using admin_domain;
using admin_domain.Entities;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class UpdateProjectCommandHandler(IProjectRepository repository) : IUpdateProjectCommandHandler
{
    public async Task<Result<Project>> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<UpdateProjectCommandHandler>()
            .ForContext("Id", command.Id)
            .ForContext("OrgId", command.OrgId)
            .ForContext("Name", command.Name);
        log.Information("UpdateProject started");

        var model = new Project { Id = command.Id, OrgId = command.OrgId, Name = command.Name };

        var result = await repository.UpdateAsync(model, cancellationToken);

        log.Information("UpdateProject completed: {Success}", result.IsSuccess);

        return result;
    }
}