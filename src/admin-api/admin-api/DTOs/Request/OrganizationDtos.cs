using System.Text.Json.Serialization;

namespace admin_api.DTOs.Request;

public sealed class CreateOrganizationRequest
{
	[JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}

public sealed class UpdateOrganizationRequest
{
	[JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}