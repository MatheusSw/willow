using admin_application.Handlers.Interfaces.ApiKeys;
using admin_application.Interfaces;
using admin_application.Queries;

using Serilog;

namespace admin_application.Handlers.Implementations.ApiKeys;

public sealed class ValidateApiKeyQueryHandler(IApiKeyRepository apiKeyRepository) : IValidateApiKeyQueryHandler
{
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

        var valid = await apiKeyRepository.ValidateAsync(query.ApiKey, cancellationToken);

        log.Information("ValidateApiKey completed: {Valid}", valid);

        return valid;
    }
}