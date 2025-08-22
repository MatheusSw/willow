namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity representing a feature flag.
/// </summary>
public record Feature
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }

    public Project? Project { get; init; }
}