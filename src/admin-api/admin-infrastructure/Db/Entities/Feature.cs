namespace admin_infrastructure.Db.Entities;

public record Feature
{
	public Guid Id { get; init; }
	public Guid ProjectId { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }

	public Project? Project { get; init; }
}