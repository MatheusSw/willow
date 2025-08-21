using evaluation_application.Interfaces;
using evaluation_application.Queries;
using Serilog;

namespace evaluation_application.Handlers;

public sealed class ValidateApiKeyQueryHandler : IValidateApiKeyQueryHandler
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ValidateApiKeyQueryHandler(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

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



