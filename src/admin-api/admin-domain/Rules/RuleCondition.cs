namespace admin_domain.Rules;

public sealed class RuleCondition
{
	public string Attribute { get; init; } = string.Empty;
	public string Op { get; init; } = "equals";
	public string? Value { get; init; }
}

public enum MatchType
{
	All,
	Any
}