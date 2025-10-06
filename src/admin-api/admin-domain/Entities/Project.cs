namespace admin_domain.Entities;

public sealed class Project
{
	public Guid Id { get; set; }
	public Guid OrgId { get; set; }
	public string Name { get; set; } = string.Empty;
}