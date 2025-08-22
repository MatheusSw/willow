namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity storing the state of a feature in a specific environment.
/// </summary>
public record FeatureState
{
    public Guid Id { get; init; }
    public Guid FeatureId { get; init; }
    public Guid EnvironmentId { get; init; }
    public bool Enabled { get; init; }
    public string? Reason { get; init; }

    public Feature? Feature { get; init; }
    public EnvironmentEntity? Environment { get; init; }
}