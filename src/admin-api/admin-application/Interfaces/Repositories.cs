using admin_domain.Entities;

namespace admin_application.Interfaces;

using FluentResults;

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

public interface IFeatureStateRepository
{
	Task<Result<FeatureState>> CreateAsync(FeatureState featureState, CancellationToken cancellationToken);
	Task<Result<FeatureState>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<List<FeatureState>>> ListAsync(Guid? featureId, Guid? environmentId, CancellationToken cancellationToken);
	Task<Result<FeatureState>> UpdateAsync(FeatureState featureState, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IOrganizationRepository
{
	Task<Result<Organization>> CreateAsync(Organization organization, CancellationToken cancellationToken);
	Task<Result<Organization>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<List<Organization>>> ListAsync(string? name, CancellationToken cancellationToken);
	Task<Result<Organization>> UpdateAsync(Organization organization, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IRuleRepository
{
	Task<Result<Rule>> CreateAsync(Rule rule, CancellationToken cancellationToken);
	Task<Result<Rule>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<List<Rule>>> ListAsync(Guid? featureId, Guid? environmentId, CancellationToken cancellationToken);
	Task<Result<Rule>> UpdateAsync(Rule rule, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IEnvironmentRepository
{
	Task<Result<admin_domain.Entities.Environment>> CreateAsync(admin_domain.Entities.Environment environment, CancellationToken cancellationToken);
	Task<Result<admin_domain.Entities.Environment>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<List<admin_domain.Entities.Environment>>> ListAsync(Guid? projectId, CancellationToken cancellationToken);
	Task<Result<admin_domain.Entities.Environment>> UpdateAsync(admin_domain.Entities.Environment environment, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public interface IApiKeyRepository
{
	Task<bool> ValidateAsync(string apiKey, CancellationToken cancellationToken);
}