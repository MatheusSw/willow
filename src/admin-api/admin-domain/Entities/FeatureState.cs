namespace admin_domain.Entities;

public sealed class FeatureState
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public Guid EnvironmentId { get; set; }
    public bool Enabled { get; set; }
    public string Reason { get; set; } = string.Empty;
}