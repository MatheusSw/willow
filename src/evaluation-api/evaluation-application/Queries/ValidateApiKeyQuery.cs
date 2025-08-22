namespace evaluation_application.Queries;

/// <summary>
/// Query to validate an API key.
/// </summary>
public sealed class ValidateApiKeyQuery
{
    /// <summary>
    /// Raw API key provided by the caller.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;
}

/// <summary>
/// Handles API key validation queries.
/// </summary>
public interface IValidateApiKeyQueryHandler
{
    /// <summary>
    /// Validates the provided API key.
    /// </summary>
    /// <param name="query">API key query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the key is valid; otherwise false.</returns>
    Task<bool> HandleAsync(ValidateApiKeyQuery query, CancellationToken cancellationToken);
}



