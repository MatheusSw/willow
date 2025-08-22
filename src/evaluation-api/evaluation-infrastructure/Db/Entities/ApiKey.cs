namespace evaluation_infrastructure.Db.Entities;

/// <summary>
/// Represents an API key allowed to access the evaluation API.
/// </summary>
public record ApiKey
{
    public Guid Id { get; init; }
    public Guid OrgId { get; init; }
    public Guid? ProjectId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string[] Scopes { get; init; } = Array.Empty<string>();
    public string HashedKey { get; init; } = string.Empty;
    public bool Active { get; init; } = true;

    public Organization? Organization { get; init; }
    public Project? Project { get; init; }
}