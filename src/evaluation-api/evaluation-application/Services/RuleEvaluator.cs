using System.Collections.Generic;
using System.Linq;
using evaluation_application.Interfaces;
using evaluation_domain.Rules;
using Serilog;

namespace evaluation_application.Services;

/// <summary>
/// Provides rule-based evaluation for feature flags.
/// </summary>
public static class RuleEvaluator
{
    /// <summary>
    /// Evaluates the provided feature configuration against caller attributes to determine enablement and reason.
    /// </summary>
    /// <param name="config">Feature configuration including default and rules.</param>
    /// <param name="attributes">Caller attributes used to match rule conditions.</param>
    /// <returns>A tuple with Enabled and Reason.</returns>
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

    /// <summary>
    /// Evaluates a single rule condition against the provided attributes.
    /// </summary>
    /// <param name="condition">The rule condition to evaluate.</param>
    /// <param name="attributes">Caller attributes.</param>
    /// <returns>True when the condition is satisfied.</returns>
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


