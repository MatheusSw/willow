using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateFeatureStateRequest
{
    [JsonPropertyName("featureId")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environmentId")] public Guid EnvironmentId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("reason")] public string Reason { get; init; } = string.Empty;
}

public sealed class UpdateFeatureStateRequest
{
    [JsonPropertyName("featureId")] public Guid FeatureId { get; init; }
    [JsonPropertyName("environmentId")] public Guid EnvironmentId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("reason")] public string Reason { get; init; } = string.Empty;
}


