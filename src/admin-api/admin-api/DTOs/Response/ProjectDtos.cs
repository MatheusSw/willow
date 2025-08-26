using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class ProjectResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("org_id")] public Guid OrgId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}