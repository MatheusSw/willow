using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class RuleConditionDto
{
    [JsonPropertyName("attribute")] public string Attribute { get; init; } = string.Empty;
    [JsonPropertyName("op")] public string Op { get; init; } = "equals";
    [JsonPropertyName("value")] public string? Value { get; init; }
}

public sealed class CreateRuleRequest
{
    [JsonPropertyName("featureId")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environmentId")] public Guid? EnvironmentId { get; init; }
    [JsonPropertyName("priority")] public int Priority { get; init; }
    [JsonPropertyName("match_type")] public string MatchType { get; init; } = "all";
    [JsonPropertyName("conditions")] public List<RuleConditionDto> Conditions { get; init; } = new();
}

public sealed class UpdateRuleRequest
{
    [JsonPropertyName("featureId")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environmentId")] public Guid? EnvironmentId { get; init; }
    [JsonPropertyName("priority")] public int Priority { get; init; }
    [JsonPropertyName("match_type")] public string MatchType { get; init; } = "all";
    [JsonPropertyName("conditions")] public List<RuleConditionDto> Conditions { get; init; } = new();
}


