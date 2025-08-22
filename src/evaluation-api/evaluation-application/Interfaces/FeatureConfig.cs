using System;
using System.Collections.Generic;
using evaluation_domain.Rules;

namespace evaluation_application.Interfaces;

public sealed class FeatureConfig
{
    public bool Enabled { get; init; }
    public string? Reason { get; init; }
    public List<FeatureRule> Rules { get; init; } = new();
    public DateTimeOffset UpdatedAt { get; init; }
}

public sealed class FeatureRule
{
    public string MatchType { get; init; } = "all";
    public int Priority { get; init; }
    public List<RuleCondition> Conditions { get; init; } = new();
}



