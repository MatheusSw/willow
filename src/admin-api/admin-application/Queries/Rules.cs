namespace admin_application.Queries;

public sealed class GetRuleByIdQuery
{
	public Guid Id { get; init; }
}

public sealed class ListRulesQuery
{
	public Guid? FeatureId { get; init; }
	public Guid? EnvironmentId { get; init; }
}