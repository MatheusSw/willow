namespace admin_infrastructure.Db.Entities;

public record ApiKey
{
    public Guid Id { get; init; }
    public Guid OrgId { get; init; }
    public Guid? ProjectId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string[] Scopes { get; init; } = [];
    public string HashedKey { get; init; } = string.Empty;
    public bool Active { get; init; } = true;

    public Organization? Organization { get; init; }
    public Project? Project { get; init; }
}


