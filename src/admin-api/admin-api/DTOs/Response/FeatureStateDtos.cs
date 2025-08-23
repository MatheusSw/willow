using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class FeatureStateResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("feature_id")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environment_id")] public Guid EnvironmentId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("reason")] public string Reason { get; init; } = string.Empty;
}


