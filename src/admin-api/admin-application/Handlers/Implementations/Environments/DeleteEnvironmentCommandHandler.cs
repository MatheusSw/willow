using admin_application.Commands;
using admin_application.Handlers.Interfaces.Environments;
using admin_application.Interfaces;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Environments;

public sealed class DeleteEnvironmentCommandHandler : IDeleteEnvironmentCommandHandler
{
    private readonly IEnvironmentRepository _repository;
    public DeleteEnvironmentCommandHandler(IEnvironmentRepository repository) { _repository = repository; }

    public async Task<Result> HandleAsync(DeleteEnvironmentCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<DeleteEnvironmentCommandHandler>()
            .ForContext("Id", command.Id);
        log.Information("DeleteEnvironment started");
        var result = await _repository.DeleteAsync(command.Id, cancellationToken);
        log.Information("DeleteEnvironment completed: {Success}", result.IsSuccess);
        return result;
    }
}


