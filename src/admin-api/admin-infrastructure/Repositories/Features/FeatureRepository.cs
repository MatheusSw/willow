using admin_application.Interfaces;

using admin_domain.Entities;

using admin_infrastructure.Db;

using FluentResults;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace admin_infrastructure.Repositories.Features;

public sealed class FeatureRepository(FeatureToggleDbContext dbContext) : IFeatureRepository
{
	public async Task<Result<Feature>> CreateAsync(Feature feature, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureRepository>()
			.ForContext("ProjectId", feature.ProjectId)
			.ForContext("Name", feature.Name);

		log.Information("Feature Create started");

		try
		{
			var entity = new Db.Entities.Feature { Id = feature.Id, ProjectId = feature.ProjectId, Name = feature.Name, Description = feature.Description };
			dbContext.Features.Add(entity);
			await dbContext.SaveChangesAsync(cancellationToken);
			log.Information("Feature Create completed");
			return Result.Ok(feature);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Feature Create failed");
			return Result.Fail("Failed to create feature");
		}
	}

	public async Task<Result<Feature>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureRepository>()
			.ForContext("Id", id);

		log.Information("Feature GetById started");

		var entity = await dbContext.Features.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Feature not found");
			return Result.Fail("NotFound");
		}

		var model = new Feature { Id = entity.Id, ProjectId = entity.ProjectId, Name = entity.Name, Description = entity.Description };
		log.Information("Feature GetById completed");
		return Result.Ok(model);
	}

	public async Task<Result<List<Feature>>> ListAsync(Guid? projectId, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureRepository>()
			.ForContext("ProjectId", projectId);

		log.Information("Feature List started");

		var query = dbContext.Features.AsNoTracking().AsQueryable();
		if (projectId.HasValue)
		{
			query = query.Where(f => f.ProjectId == projectId.Value);
		}

		var result = await query
			.OrderBy(f => f.Name)
			.Select(f => new Feature { Id = f.Id, ProjectId = f.ProjectId, Name = f.Name, Description = f.Description })
			.ToListAsync(cancellationToken);

		log.Information("Feature List completed: Count={Count}", result.Count);
		return Result.Ok(result);
	}

	public async Task<Result<Feature>> UpdateAsync(Feature feature, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureRepository>()
			.ForContext("Id", feature.Id)
			.ForContext("ProjectId", feature.ProjectId)
			.ForContext("Name", feature.Name);

		log.Information("Feature Update started");

		try
		{
			var affected = await dbContext.Features
				.Where(f => f.Id == feature.Id)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(f => f.ProjectId, feature.ProjectId)
					.SetProperty(f => f.Name, feature.Name)
					.SetProperty(f => f.Description, feature.Description), cancellationToken);

			if (affected == 0)
			{
				log.Information("Feature to update not found");
				return Result.Fail("NotFound");
			}

			log.Information("Feature Update completed");
			return Result.Ok(feature);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Feature Update failed");
			return Result.Fail("Failed to update feature");
		}
	}

	public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<FeatureRepository>()
			.ForContext("Id", id);

		log.Information("Feature Delete started");

		var entity = await dbContext.Features.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Feature to delete not found");
			return Result.Fail("NotFound");
		}

		dbContext.Features.Remove(entity);

		try
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			log.Information("Feature Delete completed");
			return Result.Ok();
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Feature Delete failed");
			return Result.Fail("Failed to delete feature");
		}
	}
}