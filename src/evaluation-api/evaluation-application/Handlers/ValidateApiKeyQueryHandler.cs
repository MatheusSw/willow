using evaluation_application.Interfaces;
using evaluation_application.Queries;
using Serilog;

namespace evaluation_application.Handlers;

/// <summary>
/// Handles API key validation against persistence and cache.
/// </summary>
public sealed class ValidateApiKeyQueryHandler : IValidateApiKeyQueryHandler
{
    private readonly IApiKeyRepository _apiKeyRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidateApiKeyQueryHandler"/>.
    /// </summary>
    /// <param name="apiKeyRepository">Repository used to validate API keys.</param>
    public ValidateApiKeyQueryHandler(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    /// <inheritdoc />
    public async Task<bool> HandleAsync(ValidateApiKeyQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<ValidateApiKeyQueryHandler>()
            .ForContext("ApiKeyPresent", !string.IsNullOrWhiteSpace(query.ApiKey));
        
        log.Information("ValidateApiKey started");

        if (string.IsNullOrWhiteSpace(query.ApiKey))
        {
            log.Information("API key missing");
            
            return false;
        }
        
        var valid = await _apiKeyRepository.ValidateAsync(query.ApiKey, cancellationToken);
        
        log.Information("ValidateApiKey completed: {Valid}", valid);
        
        return valid;
    }
}



