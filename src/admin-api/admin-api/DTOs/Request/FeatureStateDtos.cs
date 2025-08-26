using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateFeatureStateRequest
{
    [JsonPropertyName("feature_id")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environment_id")] public Guid EnvironmentId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("reason")] public string Reason { get; init; } = string.Empty;
}

public sealed class UpdateFeatureStateRequest
{
    [JsonPropertyName("feature_id")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environment_id")] public Guid EnvironmentId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("reason")] public string Reason { get; init; } = string.Empty;
}