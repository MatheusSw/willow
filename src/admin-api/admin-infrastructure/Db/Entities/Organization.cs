namespace admin_infrastructure.Db.Entities;

public record Organization
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
}