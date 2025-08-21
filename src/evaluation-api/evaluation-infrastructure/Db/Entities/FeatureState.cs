namespace evaluation_infrastructure.Db.Entities;

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