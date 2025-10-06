namespace admin_domain.Entities;

public sealed class Feature
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
}