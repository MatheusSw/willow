using admin_application.Interfaces;
using admin_domain.Entities;
using admin_infrastructure.Db;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace admin_infrastructure.Repositories.FeatureStates;

public sealed class FeatureStateRepository : IFeatureStateRepository
{
    private readonly FeatureToggleDbContext _dbContext;

    public FeatureStateRepository(FeatureToggleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<FeatureState>> CreateAsync(FeatureState featureState, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureStateRepository>()
            .ForContext("FeatureId", featureState.FeatureId)
            .ForContext("EnvironmentId", featureState.EnvironmentId)
            .ForContext("Enabled", featureState.Enabled);

        log.Information("FeatureState Create started");

        try
        {
            var entity = new Db.Entities.FeatureState { Id = featureState.Id, FeatureId = featureState.FeatureId, EnvironmentId = featureState.EnvironmentId, Enabled = featureState.Enabled, Reason = featureState.Reason };
            _dbContext.FeatureStates.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            log.Information("FeatureState Create completed");
            return Result.Ok(featureState);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "FeatureState Create failed");
            return Result.Fail("Failed to create feature state");
        }
    }

    public async Task<Result<FeatureState>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureStateRepository>()
            .ForContext("Id", id);

        log.Information("FeatureState GetById started");

        var entity = await _dbContext.FeatureStates.AsNoTracking().FirstOrDefaultAsync(fs => fs.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("FeatureState not found");
            return Result.Fail("NotFound");
        }

        var model = new FeatureState { Id = entity.Id, FeatureId = entity.FeatureId, EnvironmentId = entity.EnvironmentId, Enabled = entity.Enabled, Reason = entity.Reason ?? string.Empty };
        log.Information("FeatureState GetById completed");
        return Result.Ok(model);
    }

    public async Task<Result<List<FeatureState>>> ListAsync(Guid? featureId, Guid? environmentId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureStateRepository>()
            .ForContext("FeatureId", featureId)
            .ForContext("EnvironmentId", environmentId);

        log.Information("FeatureState List started");

        var query = _dbContext.FeatureStates.AsNoTracking().AsQueryable();
        if (featureId.HasValue)
        {
            query = query.Where(fs => fs.FeatureId == featureId.Value);
        }
        if (environmentId.HasValue)
        {
            query = query.Where(fs => fs.EnvironmentId == environmentId.Value);
        }

        var result = await query
            .OrderBy(fs => fs.FeatureId)
            .ThenBy(fs => fs.EnvironmentId)
            .Select(fs => new FeatureState { Id = fs.Id, FeatureId = fs.FeatureId, EnvironmentId = fs.EnvironmentId, Enabled = fs.Enabled, Reason = fs.Reason ?? string.Empty })
            .ToListAsync(cancellationToken);

        log.Information("FeatureState List completed: Count={Count}", result.Count);
        return Result.Ok(result);
    }

    public async Task<Result<FeatureState>> UpdateAsync(FeatureState featureState, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureStateRepository>()
            .ForContext("Id", featureState.Id)
            .ForContext("FeatureId", featureState.FeatureId)
            .ForContext("EnvironmentId", featureState.EnvironmentId)
            .ForContext("Enabled", featureState.Enabled);

        log.Information("FeatureState Update started");

        try
        {
            var affected = await _dbContext.FeatureStates
                .Where(fs => fs.Id == featureState.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(fs => fs.FeatureId, featureState.FeatureId)
                    .SetProperty(fs => fs.EnvironmentId, featureState.EnvironmentId)
                    .SetProperty(fs => fs.Enabled, featureState.Enabled)
                    .SetProperty(fs => fs.Reason, featureState.Reason), cancellationToken);

            if (affected == 0)
            {
                log.Information("FeatureState to update not found");
                return Result.Fail("NotFound");
            }

            log.Information("FeatureState Update completed");
            return Result.Ok(featureState);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "FeatureState Update failed");
            return Result.Fail("Failed to update feature state");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureStateRepository>()
            .ForContext("Id", id);

        log.Information("FeatureState Delete started");

        var entity = await _dbContext.FeatureStates.FirstOrDefaultAsync(fs => fs.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("FeatureState to delete not found");
            return Result.Fail("NotFound");
        }

        _dbContext.FeatureStates.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            log.Information("FeatureState Delete completed");
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "FeatureState Delete failed");
            return Result.Fail("Failed to delete feature state");
        }
    }
}


