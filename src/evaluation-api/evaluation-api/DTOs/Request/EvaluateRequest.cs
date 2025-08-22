using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace evaluation_api.DTOs.Request;

/// <summary>
/// Request payload for evaluating a feature flag for a given project/environment and caller attributes.
/// </summary>
public sealed class EvaluateRequest
{
    /// <summary>
    /// The project identifier.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("project")]
    public string Project { get; init; } = string.Empty;

    /// <summary>
    /// The environment identifier (e.g., dev, staging, prod).
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("environment")]
    public string Environment { get; init; } = string.Empty;

    /// <summary>
    /// The feature flag name to evaluate.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("feature")]
    public string Feature { get; init; } = string.Empty;

    /// <summary>
    /// Arbitrary key/value attributes describing the caller (e.g., orgId, userId).
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("attributes")]
    public Dictionary<string, string> Attributes { get; init; } = new();
}



