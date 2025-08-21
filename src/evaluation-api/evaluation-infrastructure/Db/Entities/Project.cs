namespace evaluation_infrastructure.Db.Entities;

public record Project
{
    public Guid Id { get; init; }
    public Guid OrgId { get; init; }
    public string Name { get; init; } = string.Empty;

    public Organization? Organization { get; init; }
}