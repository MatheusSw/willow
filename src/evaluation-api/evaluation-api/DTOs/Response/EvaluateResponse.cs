namespace evaluation_api.DTOs.Response;

public sealed class EvaluateResponse
{
    public bool Enabled { get; init; }
    public string? Reason { get; init; }
    public string? Variant { get; init; }
}



