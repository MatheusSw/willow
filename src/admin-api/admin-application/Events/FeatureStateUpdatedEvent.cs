using System.Text.Json.Serialization;

namespace admin_application.Events;

public sealed class FeatureStateUpdatedEvent
{
	[JsonPropertyName("project_id")]
	public string ProjectId { get; init; } = string.Empty;

	[JsonPropertyName("environment")]
	public string Environment { get; init; } = string.Empty;

	[JsonPropertyName("feature")]
	public string Feature { get; init; } = string.Empty;

	[JsonPropertyName("enabled")]
	public bool Enabled { get; init; }

	[JsonPropertyName("timestamp")]
	public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
