namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity representing a deployment environment for a project.
/// </summary>
public record EnvironmentEntity
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Key { get; init; } = string.Empty;

    public Project? Project { get; init; }
}