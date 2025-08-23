using admin_domain;
using admin_domain.Entities;

using FluentResults;

namespace admin_application.Queries;

public sealed class GetProjectByIdQuery
{
	public Guid Id { get; init; }
}

public sealed class ListProjectsQuery
{
	public Guid? OrgId { get; init; }
}

public interface IGetProjectByIdQueryHandler
{
	Task<Result<Project>> HandleAsync(GetProjectByIdQuery query, CancellationToken cancellationToken);
}

public interface IListProjectsQueryHandler
{
	Task<Result<List<Project>>> HandleAsync(ListProjectsQuery query, CancellationToken cancellationToken);
}