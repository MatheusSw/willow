namespace admin_infrastructure.Db.Entities;

public record EnvironmentEntity
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Key { get; init; } = string.Empty;

    public Project? Project { get; init; }
}