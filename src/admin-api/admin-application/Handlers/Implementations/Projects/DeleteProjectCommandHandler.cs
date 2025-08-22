using admin_application.Commands;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Interfaces;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class DeleteProjectCommandHandler : IDeleteProjectCommandHandler
{
    private readonly IProjectRepository _repository;
    public DeleteProjectCommandHandler(IProjectRepository repository) { _repository = repository; }

    public async Task<Result> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<DeleteProjectCommandHandler>()
            .ForContext("Id", command.Id);
        log.Information("DeleteProject started");
        var result = await _repository.DeleteAsync(command.Id, cancellationToken);
        log.Information("DeleteProject completed: {Success}", result.IsSuccess);
        return result;
    }
}


