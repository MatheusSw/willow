namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Database entity representing an organization.
/// </summary>
public record Organization
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}