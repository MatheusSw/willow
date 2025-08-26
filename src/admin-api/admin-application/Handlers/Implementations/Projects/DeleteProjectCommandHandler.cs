using admin_application.Commands;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Interfaces;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Projects;

public sealed class DeleteProjectCommandHandler(IProjectRepository repository) : IDeleteProjectCommandHandler
{
    public async Task<Result> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<DeleteProjectCommandHandler>()
            .ForContext("Id", command.Id);
        log.Information("DeleteProject started");

        var result = await repository.DeleteAsync(command.Id, cancellationToken);

        log.Information("DeleteProject completed: {Success}", result.IsSuccess);

        return result;
    }
}