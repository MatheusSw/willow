using admin_application.Queries;

namespace admin_application.Handlers.Interfaces.ApiKeys;

public interface IValidateApiKeyQueryHandler
{
    Task<bool> HandleAsync(ValidateApiKeyQuery query, CancellationToken cancellationToken);
}