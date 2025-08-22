using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class RuleConditionResponse
{
    [JsonPropertyName("attribute")] public string Attribute { get; init; } = string.Empty;
    [JsonPropertyName("op")] public string Op { get; init; } = "equals";
    [JsonPropertyName("value")] public string? Value { get; init; }
}

public sealed class RuleResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("feature_id")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environment_id")] public Guid? EnvironmentId { get; init; }
    [JsonPropertyName("priority")] public int Priority { get; init; }
    [JsonPropertyName("match_type")] public string MatchType { get; init; } = "all";
    [JsonPropertyName("conditions")] public List<RuleConditionResponse> Conditions { get; init; } = new();
}


