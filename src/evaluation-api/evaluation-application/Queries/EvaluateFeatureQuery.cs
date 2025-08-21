namespace evaluation_application.Queries;

public sealed class EvaluateFeatureQuery
{
    public string Project { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public string Feature { get; init; } = string.Empty;
    public IDictionary<string, string> Attributes { get; init; } = new Dictionary<string, string>();
}

public sealed class EvaluateFeatureResult
{
    public bool Enabled { get; init; }
    public string? Reason { get; init; }
    public string? Variant { get; init; }
}

public interface IEvaluateFeatureQueryHandler
{
    Task<EvaluateFeatureResult> HandleAsync(EvaluateFeatureQuery query, CancellationToken cancellationToken);
}



