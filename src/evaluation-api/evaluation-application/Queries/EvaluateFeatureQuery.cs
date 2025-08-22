namespace evaluation_application.Queries;

/// <summary>
/// Query parameters for evaluating a feature flag in the application layer.
/// </summary>
public sealed class EvaluateFeatureQuery
{
    /// <summary>
    /// Project identifier.
    /// </summary>
    public string Project { get; init; } = string.Empty;
    /// <summary>
    /// Environment identifier.
    /// </summary>
    public string Environment { get; init; } = string.Empty;
    /// <summary>
    /// Feature flag name.
    /// </summary>
    public string Feature { get; init; } = string.Empty;
    /// <summary>
    /// Caller attributes keyed by attribute name.
    /// </summary>
    public IDictionary<string, string> Attributes { get; init; } = new Dictionary<string, string>();
}

/// <summary>
/// Result of a feature flag evaluation.
/// </summary>
public sealed class EvaluateFeatureResult
{
    /// <summary>
    /// Whether the feature is enabled.
    /// </summary>
    public bool Enabled { get; init; }
    /// <summary>
    /// Explanation describing why the decision was made.
    /// </summary>
    public string? Reason { get; init; }
    /// <summary>
    /// Optional selected variant.
    /// </summary>
    public string? Variant { get; init; }
}

/// <summary>
/// Handles feature evaluation queries.
/// </summary>
public interface IEvaluateFeatureQueryHandler
{
    /// <summary>
    /// Executes the evaluation for the given query.
    /// </summary>
    /// <param name="query">The evaluation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The evaluation result.</returns>
    Task<EvaluateFeatureResult> HandleAsync(EvaluateFeatureQuery query, CancellationToken cancellationToken);
}



