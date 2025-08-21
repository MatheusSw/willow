using System.Collections.Generic;
using System.Linq;
using evaluation_application.Interfaces;
using evaluation_domain.Rules;
using Serilog;

namespace evaluation_application.Services;

public static class RuleEvaluator
{
    public static (bool Enabled, string Reason) Evaluate(FeatureConfig config, IDictionary<string, string> attributes)
    {
        var log = Log.ForContext(typeof(RuleEvaluator));
        log.Information("Rule evaluation started");

        var orderedRules = config.Rules.OrderBy(r => r.Priority);

        foreach (var rule in orderedRules)
        {
            var isMatch = rule.MatchType == "any"
                ? rule.Conditions.Any(c => EvaluateCondition(c, attributes))
                : rule.Conditions.All(c => EvaluateCondition(c, attributes));

            if (isMatch)
            {
                var first = rule.Conditions.FirstOrDefault();
                var reason = first is null ? "rule:match" : $"rule: {first.Attribute}={first.Value}";
                log.Information("Rule matched: Priority={Priority} Reason={Reason}", rule.Priority, reason);
                return (true, reason);
            }
        }

        var defaultReason = string.IsNullOrWhiteSpace(config.Reason)
            ? $"default:{config.Enabled.ToString().ToLowerInvariant()}"
            : config.Reason!;
        log.Information("Rule evaluation completed: DefaultReason={Reason}", defaultReason);
        return (config.Enabled, defaultReason);
    }

    private static bool EvaluateCondition(RuleCondition condition, IDictionary<string, string> attributes)
    {
        attributes.TryGetValue(condition.Attribute, out var attributeValue);

        switch (condition.Op)
        {
            case "equals":
                return string.Equals(attributeValue, condition.Value, System.StringComparison.OrdinalIgnoreCase);
            default:
                return false;
        }
    }
}


