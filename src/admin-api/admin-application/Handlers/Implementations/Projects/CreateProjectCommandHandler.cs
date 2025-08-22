using admin_application.Commands;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Interfaces;
using admin_domain;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class CreateProjectCommandHandler : ICreateProjectCommandHandler
{
    private readonly IProjectRepository _repository;
    public CreateProjectCommandHandler(IProjectRepository repository) { _repository = repository; }

    public async Task<Result<Project>> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<CreateProjectCommandHandler>()
            .ForContext("OrgId", command.OrgId)
            .ForContext("Name", command.Name);
        log.Information("CreateProject started");
        var model = new Project { Id = Guid.NewGuid(), OrgId = command.OrgId, Name = command.Name };
        var result = await _repository.CreateAsync(model, cancellationToken);
        log.Information("CreateProject completed: {Success}", result.IsSuccess);
        return result;
    }
}


