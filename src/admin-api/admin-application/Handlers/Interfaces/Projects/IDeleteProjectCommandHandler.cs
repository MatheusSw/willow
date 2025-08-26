using admin_application.Commands;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Projects;

public interface IDeleteProjectCommandHandler
{
    Task<Result> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken);
}