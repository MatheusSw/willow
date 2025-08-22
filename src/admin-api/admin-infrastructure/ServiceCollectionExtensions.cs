using admin_application.Interfaces;
using admin_infrastructure.Repositories.Projects;
using admin_infrastructure.Repositories.Features;
using admin_infrastructure.Repositories.FeatureStates;
using admin_infrastructure.Repositories.Organizations;
using admin_infrastructure.Repositories.Security;
using admin_infrastructure.Repositories.Rules;
using admin_infrastructure.Repositories.Environments;
using Microsoft.Extensions.DependencyInjection;

namespace admin_infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IFeatureStateRepository, FeatureStateRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        return services;
    }
}


