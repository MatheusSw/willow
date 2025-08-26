using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class FeatureResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("project_id")] public Guid ProjectId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }
}