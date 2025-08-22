using admin_application.Interfaces;
using admin_infrastructure.Repositories.Projects;
using admin_infrastructure.Repositories.Features;
using admin_infrastructure.Repositories.Security;
using Microsoft.Extensions.DependencyInjection;

namespace admin_infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        return services;
    }
}


