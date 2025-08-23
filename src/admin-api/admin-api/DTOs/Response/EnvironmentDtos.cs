using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class EnvironmentResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("project_id")] public Guid ProjectId { get; init; }
    [JsonPropertyName("key")] public string Key { get; init; } = string.Empty;
}


