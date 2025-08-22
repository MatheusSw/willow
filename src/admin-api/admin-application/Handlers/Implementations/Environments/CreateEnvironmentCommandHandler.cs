using admin_application.Commands;
using admin_application.Handlers.Interfaces.Environments;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Environments;

public sealed class CreateEnvironmentCommandHandler : ICreateEnvironmentCommandHandler
{
    private readonly IEnvironmentRepository _repository;
    public CreateEnvironmentCommandHandler(IEnvironmentRepository repository) { _repository = repository; }

    public async Task<Result<admin_domain.Entities.Environment>> HandleAsync(CreateEnvironmentCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<CreateEnvironmentCommandHandler>()
            .ForContext("ProjectId", command.ProjectId)
            .ForContext("Key", command.Key);
        log.Information("CreateEnvironment started");
        var model = new admin_domain.Entities.Environment { Id = Guid.NewGuid(), ProjectId = command.ProjectId, Key = command.Key };
        var result = await _repository.CreateAsync(model, cancellationToken);
        log.Information("CreateEnvironment completed: {Success}", result.IsSuccess);
        return result;
    }
}


