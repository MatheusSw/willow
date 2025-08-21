using evaluation_application.Handlers;
using evaluation_application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace evaluation_application;

public static class DependencyInjection
{
    public static IServiceCollection AddEvaluationApplication(this IServiceCollection services)
    {
        services.AddScoped<IEvaluateFeatureQueryHandler, EvaluateFeatureQueryHandler>();
        services.AddScoped<IValidateApiKeyQueryHandler, ValidateApiKeyQueryHandler>();

        return services;
    }
}


