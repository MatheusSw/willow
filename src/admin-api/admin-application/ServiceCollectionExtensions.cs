using admin_application.Queries;
using admin_application.Commands;
using admin_application.Handlers;
using admin_application.Handlers.Implementations.ApiKeys;
using admin_application.Handlers.Implementations.Projects;
using admin_application.Handlers.Implementations.Features;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Handlers.Implementations.Rules;
using admin_application.Handlers.Implementations.Environments;
using admin_application.Handlers.Implementations.Organizations;
using admin_application.Handlers.Interfaces.ApiKeys;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Handlers.Interfaces.Rules;
using admin_application.Handlers.Interfaces.Environments;
using admin_application.Handlers.Interfaces.Organizations;
using Microsoft.Extensions.DependencyInjection;

namespace admin_application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidateApiKeyQueryHandler, ValidateApiKeyQueryHandler>();

        services.AddScoped<ICreateProjectCommandHandler, CreateProjectCommandHandler>();
        services.AddScoped<IUpdateProjectCommandHandler, UpdateProjectCommandHandler>();
        services.AddScoped<IDeleteProjectCommandHandler, DeleteProjectCommandHandler>();
        services.AddScoped<IGetProjectByIdQueryHandler, GetProjectByIdQueryHandler>();
        services.AddScoped<IListProjectsQueryHandler, ListProjectsQueryHandler>();

        services.AddScoped<ICreateFeatureCommandHandler, CreateFeatureCommandHandler>();
        services.AddScoped<IUpdateFeatureCommandHandler, UpdateFeatureCommandHandler>();
        services.AddScoped<IDeleteFeatureCommandHandler, DeleteFeatureCommandHandler>();
        services.AddScoped<IGetFeatureByIdQueryHandler, GetFeatureByIdQueryHandler>();
        services.AddScoped<IListFeaturesQueryHandler, ListFeaturesQueryHandler>();

        services.AddScoped<ICreateFeatureStateCommandHandler, CreateFeatureStateCommandHandler>();
        services.AddScoped<IUpdateFeatureStateCommandHandler, UpdateFeatureStateCommandHandler>();
        services.AddScoped<IDeleteFeatureStateCommandHandler, DeleteFeatureStateCommandHandler>();
        services.AddScoped<IGetFeatureStateByIdQueryHandler, GetFeatureStateByIdQueryHandler>();
        services.AddScoped<IListFeatureStatesQueryHandler, ListFeatureStatesQueryHandler>();

        services.AddScoped<ICreateOrganizationCommandHandler, CreateOrganizationCommandHandler>();
        services.AddScoped<IUpdateOrganizationCommandHandler, UpdateOrganizationCommandHandler>();
        services.AddScoped<IDeleteOrganizationCommandHandler, DeleteOrganizationCommandHandler>();
        services.AddScoped<IGetOrganizationByIdQueryHandler, GetOrganizationByIdQueryHandler>();
        services.AddScoped<IListOrganizationsQueryHandler, ListOrganizationsQueryHandler>();

        services.AddScoped<ICreateRuleCommandHandler, CreateRuleCommandHandler>();
        services.AddScoped<IUpdateRuleCommandHandler, UpdateRuleCommandHandler>();
        services.AddScoped<IDeleteRuleCommandHandler, DeleteRuleCommandHandler>();
        services.AddScoped<IGetRuleByIdQueryHandler, GetRuleByIdQueryHandler>();
        services.AddScoped<IListRulesQueryHandler, ListRulesQueryHandler>();

        services.AddScoped<ICreateEnvironmentCommandHandler, CreateEnvironmentCommandHandler>();
        services.AddScoped<IUpdateEnvironmentCommandHandler, UpdateEnvironmentCommandHandler>();
        services.AddScoped<IDeleteEnvironmentCommandHandler, DeleteEnvironmentCommandHandler>();
        services.AddScoped<IGetEnvironmentByIdQueryHandler, GetEnvironmentByIdQueryHandler>();
        services.AddScoped<IListEnvironmentsQueryHandler, ListEnvironmentsQueryHandler>();
        return services;
    }
}


