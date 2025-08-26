using admin_application.Interfaces;
using admin_infrastructure.Models.Redis;
using admin_infrastructure.Repositories.Environments;
using admin_infrastructure.Repositories.Features;
using admin_infrastructure.Repositories.FeatureStates;
using admin_infrastructure.Repositories.Organizations;
using admin_infrastructure.Repositories.Projects;
using admin_infrastructure.Repositories.Rules;
using admin_infrastructure.Repositories.Security;
using admin_infrastructure.Services.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

namespace admin_infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IFeatureStateRepository, FeatureStateRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();

        // Redis connection multiplexer
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>().Value;

            var options = ConfigurationOptions.Parse(redisSettings.Configuration ?? "localhost:6379");
            options.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(options);
        });

        // Redis services
        services.AddScoped<IEventPublisher, RedisEventPublisher>();
        services.AddScoped<ICacheInvalidator, RedisCacheInvalidator>();

        return services;
    }
}