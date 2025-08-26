namespace admin_application.Commands;

public sealed class CreateProjectCommand
{
    public Guid OrgId { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class UpdateProjectCommand
{
    public Guid Id { get; init; }
    public Guid OrgId { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class DeleteProjectCommand
{
    public Guid Id { get; init; }
}