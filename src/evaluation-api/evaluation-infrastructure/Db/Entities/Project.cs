namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity representing a project within an organization.
/// </summary>
public record Project
{
    public Guid Id { get; init; }
    public Guid OrgId { get; init; }
    public string Name { get; init; } = string.Empty;

    public Organization? Organization { get; init; }
}