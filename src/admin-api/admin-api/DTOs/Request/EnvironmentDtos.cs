using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateEnvironmentRequest
{
    [JsonPropertyName("project_id")] public Guid ProjectId { get; init; }
    [JsonPropertyName("key")] public string Key { get; init; } = string.Empty;
}

public sealed class UpdateEnvironmentRequest
{
    [JsonPropertyName("project_id")] public Guid ProjectId { get; init; }
    [JsonPropertyName("key")] public string Key { get; init; } = string.Empty;
}


