namespace admin_infrastructure.Db.Entities;

public record Rule
{
	public Guid Id { get; init; }
	public Guid FeatureId { get; init; }
	public Guid? EnvironmentId { get; init; }
	public int Priority { get; init; }
	public string MatchType { get; init; } = "all";
	public string ConditionsJson { get; init; } = "[]";

	public Feature? Feature { get; init; }
	public EnvironmentEntity? Environment { get; init; }
}