using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateFeatureRequest
{
    [JsonPropertyName("projectId")] public Guid ProjectId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }
}

public sealed class UpdateFeatureRequest
{
    [JsonPropertyName("projectId")] public Guid ProjectId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }
}


