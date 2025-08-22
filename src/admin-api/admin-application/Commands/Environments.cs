namespace admin_application.Commands;

public sealed class CreateEnvironmentCommand
{
    public Guid ProjectId { get; init; }
    public string Key { get; init; } = string.Empty;
}

public sealed class UpdateEnvironmentCommand
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Key { get; init; } = string.Empty;
}

public sealed class DeleteEnvironmentCommand
{
    public Guid Id { get; init; }
}


