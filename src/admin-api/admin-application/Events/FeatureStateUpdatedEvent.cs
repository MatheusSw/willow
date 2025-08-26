using System.Text.Json.Serialization;

namespace admin_application.Events;

public sealed class FeatureStateUpdatedEvent
{
    [JsonPropertyName("project_id")]
    public Guid ProjectId { get; init; }

    [JsonPropertyName("feature")]
    public string Feature { get; init; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; } = DateTime.UtcNow;
}
