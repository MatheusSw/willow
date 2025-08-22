namespace evaluation_api.DTOs.Response;

/// <summary>
/// Response payload describing the outcome of a feature evaluation.
/// </summary>
public sealed class EvaluateResponse
{
    /// <summary>
    /// Indicates whether the feature is enabled for the given attributes.
    /// </summary>
    public bool Enabled { get; init; }
    /// <summary>
    /// Reason for the evaluation outcome (e.g., matched rule or default).
    /// </summary>
    public string? Reason { get; init; }
    /// <summary>
    /// Optional variant selected when using multivariate flags.
    /// </summary>
    public string? Variant { get; init; }
}



