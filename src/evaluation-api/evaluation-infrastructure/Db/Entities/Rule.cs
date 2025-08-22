namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity describing a rule for a feature, including match type and conditions JSON.
/// </summary>
public record Rule
{
    public Guid Id { get; init; }
    public Guid FeatureId { get; init; }
    public Guid? EnvironmentId { get; init; }
    public int Priority { get; init; }
    public string MatchType { get; init; } = "all";
    public string ConditionsJson { get; init; } = "[]";

    public Feature? Feature { get; init; }
    public EnvironmentEntity? Environment { get; init; }
}