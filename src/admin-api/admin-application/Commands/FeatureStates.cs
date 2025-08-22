namespace admin_application.Commands;

public sealed class CreateFeatureStateCommand
{
    public Guid FeatureId { get; init; }
    public Guid EnvironmentId { get; init; }
    public bool Enabled { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed class UpdateFeatureStateCommand
{
    public Guid Id { get; init; }
    public Guid FeatureId { get; init; }
    public Guid EnvironmentId { get; init; }
    public bool Enabled { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed class DeleteFeatureStateCommand
{
    public Guid Id { get; init; }
}


