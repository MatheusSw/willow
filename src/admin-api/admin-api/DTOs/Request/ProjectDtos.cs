using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateProjectRequest
{
    [JsonPropertyName("orgId")] public Guid OrgId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}

public sealed class UpdateProjectRequest
{
    [JsonPropertyName("orgId")] public Guid OrgId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}


