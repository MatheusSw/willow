using admin_domain.Entities;

namespace admin_application.Interfaces;

using FluentResults;
using admin_domain;

public interface IProjectRepository
{
    Task<Result<Project>> CreateAsync(Project project, CancellationToken cancellationToken);
    Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<Project>>> ListAsync(Guid? orgId, CancellationToken cancellationToken);
    Task<Result<Project>> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IFeatureRepository
{
    Task<Result<Feature>> CreateAsync(Feature feature, CancellationToken cancellationToken);
    Task<Result<Feature>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<Feature>>> ListAsync(Guid? projectId, CancellationToken cancellationToken);
    Task<Result<Feature>> UpdateAsync(Feature feature, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IApiKeyRepository
{
    Task<bool> ValidateAsync(string apiKey, CancellationToken cancellationToken);
}
