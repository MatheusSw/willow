using admin_application.Queries;
using admin_application.Commands;
using admin_application.Handlers;
using admin_application.Handlers.Implementations.ApiKeys;
using admin_application.Handlers.Implementations.Projects;
using admin_application.Handlers.Implementations.Features;
using admin_application.Handlers.Interfaces.ApiKeys;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Handlers.Interfaces.Features;
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
        return services;
    }
}


