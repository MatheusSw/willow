namespace admin_infrastructure.Db.Entities;

public record AuditLog
{
    public Guid Id { get; init; }
    public string Actor { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string? Before { get; init; }
    public string? After { get; init; }
    public DateTimeOffset At { get; init; }
}


