using admin_domain.Rules;

namespace admin_application.Commands;

public sealed class CreateRuleCommand
{
	public Guid FeatureId { get; init; }
	public Guid? EnvironmentId { get; init; }
	public int Priority { get; init; }
	public string MatchType { get; init; } = "all";
	public List<RuleCondition> Conditions { get; init; } = [];
}

public sealed class UpdateRuleCommand
{
	public Guid Id { get; init; }
	public Guid FeatureId { get; init; }
	public Guid? EnvironmentId { get; init; }
	public int Priority { get; init; }
	public string MatchType { get; init; } = "all";
	public List<RuleCondition> Conditions { get; init; } = [];
}

public sealed class DeleteRuleCommand
{
	public Guid Id { get; init; }
}