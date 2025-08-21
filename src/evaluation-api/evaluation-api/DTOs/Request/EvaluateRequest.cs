using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace evaluation_api.DTOs.Request;

public sealed class EvaluateRequest
{
    [JsonRequired]
    [JsonPropertyName("project")]
    public string Project { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("environment")]
    public string Environment { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("feature")]
    public string Feature { get; init; } = string.Empty;

    [JsonRequired]
    [JsonPropertyName("attributes")]
    public Dictionary<string, string> Attributes { get; init; } = new();
}



