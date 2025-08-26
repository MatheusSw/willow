namespace admin_domain.Entities;

public sealed class Environment
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Key { get; set; } = string.Empty;
}