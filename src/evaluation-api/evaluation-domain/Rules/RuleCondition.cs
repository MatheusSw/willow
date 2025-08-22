namespace evaluation_domain.Rules;

/// <summary>
/// Represents a single condition within a feature rule.
/// </summary>
public sealed class RuleCondition
{
    /// <summary>
    /// Attribute name to compare (e.g., orgId, userId).
    /// </summary>
    public string Attribute { get; init; } = string.Empty;
    /// <summary>
    /// Comparison operator (e.g., "equals").
    /// </summary>
    public string Op { get; init; } = "equals";
    /// <summary>
    /// Value to compare against.
    /// </summary>
    public string? Value { get; init; }
}

/// <summary>
/// Human-friendly match type options for a rule consisting of multiple conditions.
/// </summary>
public enum MatchType
{
    All,
    Any
}



