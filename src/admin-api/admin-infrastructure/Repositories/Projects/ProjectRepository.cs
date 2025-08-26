using admin_application.Interfaces;

using admin_domain;

using admin_infrastructure.Db;
using admin_infrastructure.Db.Entities;

using FluentResults;

using Microsoft.EntityFrameworkCore;

using Serilog;

using Project = admin_domain.Entities.Project;

namespace admin_infrastructure.Repositories.Projects;

public sealed class ProjectRepository(FeatureToggleDbContext dbContext) : IProjectRepository
{
	private readonly FeatureToggleDbContext _dbContext = dbContext;

	public async Task<Result<Project>> CreateAsync(Project project, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectRepository>()
			.ForContext("OrgId", project.OrgId)
			.ForContext("Name", project.Name);

		log.Information("Project Create started");

		try
		{
			var entity = new Db.Entities.Project { Id = project.Id, OrgId = project.OrgId, Name = project.Name };

			_dbContext.Projects.Add(entity);

			await _dbContext.SaveChangesAsync(cancellationToken);

			log.Information("Project Create completed");

			return Result.Ok(project);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Project Create failed");

			return Result.Fail("Failed to create project");
		}
	}

	public async Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectRepository>()
			.ForContext("Id", id);

		log.Information("Project GetById started");

		var entity = await _dbContext.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Project not found");

			return Result.Fail("NotFound");
		}

		var model = new Project { Id = entity.Id, OrgId = entity.OrgId, Name = entity.Name };

		log.Information("Project GetById completed");

		return Result.Ok(model);
	}

	public async Task<Result<List<Project>>> ListAsync(Guid? orgId, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectRepository>()
			.ForContext("OrgId", orgId);

		log.Information("Project List started");

		var query = _dbContext.Projects.AsNoTracking().AsQueryable();
		if (orgId.HasValue)
		{
			query = query.Where(projectEntity => projectEntity.OrgId == orgId.Value);
		}

		var result = await query
			.OrderBy(projectEntity => projectEntity.Name)
			.Select(projectEntity => new Project
			{ Id = projectEntity.Id, OrgId = projectEntity.OrgId, Name = projectEntity.Name })
			.ToListAsync(cancellationToken);

		log.Information("Project List completed: Count={Count}", result.Count);

		return Result.Ok(result);
	}

	public async Task<Result<Project>> UpdateAsync(Project project, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectRepository>()
			.ForContext("Id", project.Id)
			.ForContext("OrgId", project.OrgId)
			.ForContext("Name", project.Name);

		log.Information("Project Update started");

		try
		{
			var affected = await _dbContext.Projects
				.Where(projectEntity => projectEntity.Id == project.Id)
				.ExecuteUpdateAsync(projectSetters => projectSetters
					.SetProperty(projectEntity => projectEntity.OrgId, project.OrgId)
					.SetProperty(projectEntity => projectEntity.Name, project.Name), cancellationToken);

			if (affected == 0)
			{
				log.Information("Project to update not found");

				return Result.Fail("NotFound");
			}

			log.Information("Project Update completed");

			return Result.Ok(project);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Project Update failed");

			return Result.Fail("Failed to update project");
		}
	}

	public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ProjectRepository>()
			.ForContext("Id", id);

		log.Information("Project Delete started");

		var entity = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Project to delete not found");

			return Result.Fail("NotFound");
		}

		_dbContext.Projects.Remove(entity);

		try
		{
			await _dbContext.SaveChangesAsync(cancellationToken);

			log.Information("Project Delete completed");

			return Result.Ok();
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Project Delete failed");

			return Result.Fail("Failed to delete project");
		}
	}
}