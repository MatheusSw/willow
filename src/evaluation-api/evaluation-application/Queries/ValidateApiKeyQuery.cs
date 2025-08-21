namespace evaluation_application.Queries;

public sealed class ValidateApiKeyQuery
{
    public string ApiKey { get; init; } = string.Empty;
}

public interface IValidateApiKeyQueryHandler
{
    Task<bool> HandleAsync(ValidateApiKeyQuery query, CancellationToken cancellationToken);
}



