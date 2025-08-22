using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace evaluation_api.DTOs.Request;

/// <summary>
/// Request payload for evaluating a feature flag for a given project/environment and caller attributes.
/// </summary>
public sealed class EvaluateRequest
{
    [JsonRequired]
    [JsonPropertyName("project")]
    /// <summary>
    /// The project identifier.
    /// </summary>
    public string Project { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("environment")]
    /// <summary>
    /// The environment identifier (e.g., dev, staging, prod).
    /// </summary>
    public string Environment { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("feature")]
    /// <summary>
    /// The feature flag name to evaluate.
    /// </summary>
    public string Feature { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("attributes")]
    /// <summary>
    /// Arbitrary key/value attributes describing the caller (e.g., orgId, userId).
    /// </summary>
    public Dictionary<string, string> Attributes { get; init; } = new();
}



