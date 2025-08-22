using System.Text.Json.Serialization;

namespace admin_api.DTOs.Response;

public sealed class OrganizationResponse
{
    [JsonPropertyName("id")] public Guid Id { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}


