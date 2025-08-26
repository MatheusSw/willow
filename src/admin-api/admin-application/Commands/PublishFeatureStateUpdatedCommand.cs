namespace admin_application.Commands;

public sealed class PublishFeatureStateUpdatedCommand
{
    public Guid ProjectId { get; init; }
    public string FeatureName { get; init; } = string.Empty;
    public bool Enabled { get; init; }
}
