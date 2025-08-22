using admin_application.Commands;
using admin_domain;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Projects;

public interface ICreateProjectCommandHandler
{
    Task<Result<Project>> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken);
}