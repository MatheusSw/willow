using admin_domain.Rules;

namespace admin_domain.Entities;

public sealed class Rule
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public Guid? EnvironmentId { get; set; }
    public int Priority { get; set; }
    public admin_domain.Rules.MatchType MatchType { get; set; } = admin_domain.Rules.MatchType.All;
    public List<RuleCondition> Conditions { get; set; } = new();
}