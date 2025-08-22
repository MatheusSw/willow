using admin_application.Interfaces;
using admin_infrastructure.Db;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Environment = admin_domain.Entities.Environment;

namespace admin_infrastructure.Repositories.Environments;

public sealed class EnvironmentRepository : IEnvironmentRepository
{
    private readonly FeatureToggleDbContext _dbContext;

    public EnvironmentRepository(FeatureToggleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Environment>> CreateAsync(Environment environment, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentRepository>()
            .ForContext("ProjectId", environment.ProjectId)
            .ForContext("Key", environment.Key);

        log.Information("Environment Create started");

        try
        {
            var entity = new Db.Entities.EnvironmentEntity { Id = environment.Id, ProjectId = environment.ProjectId, Key = environment.Key };
            _dbContext.Environments.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            log.Information("Environment Create completed");
            return Result.Ok(environment);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Environment Create failed");
            return Result.Fail("Failed to create environment");
        }
    }

    public async Task<Result<Environment>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentRepository>()
            .ForContext("Id", id);

        log.Information("Environment GetById started");

        var entity = await _dbContext.Environments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("Environment not found");
            return Result.Fail("NotFound");
        }

        var model = new Environment { Id = entity.Id, ProjectId = entity.ProjectId, Key = entity.Key };
        log.Information("Environment GetById completed");
        return Result.Ok(model);
    }

    public async Task<Result<List<Environment>>> ListAsync(Guid? projectId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentRepository>()
            .ForContext("ProjectId", projectId);

        log.Information("Environment List started");

        var query = _dbContext.Environments.AsNoTracking().AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(e => e.ProjectId == projectId.Value);
        }

        var result = await query
            .OrderBy(e => e.Key)
            .Select(e => new Environment { Id = e.Id, ProjectId = e.ProjectId, Key = e.Key })
            .ToListAsync(cancellationToken);

        log.Information("Environment List completed: Count={Count}", result.Count);
        return Result.Ok(result);
    }

    public async Task<Result<Environment>> UpdateAsync(Environment environment, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentRepository>()
            .ForContext("Id", environment.Id)
            .ForContext("ProjectId", environment.ProjectId)
            .ForContext("Key", environment.Key);

        log.Information("Environment Update started");

        try
        {
            var affected = await _dbContext.Environments
                .Where(e => e.Id == environment.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.ProjectId, environment.ProjectId)
                    .SetProperty(e => e.Key, environment.Key), cancellationToken);

            if (affected == 0)
            {
                log.Information("Environment to update not found");
                return Result.Fail("NotFound");
            }

            log.Information("Environment Update completed");
            return Result.Ok(environment);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Environment Update failed");
            return Result.Fail("Failed to update environment");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EnvironmentRepository>()
            .ForContext("Id", id);

        log.Information("Environment Delete started");

        var entity = await _dbContext.Environments.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("Environment to delete not found");
            return Result.Fail("NotFound");
        }

        _dbContext.Environments.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            log.Information("Environment Delete completed");
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Environment Delete failed");
            return Result.Fail("Failed to delete environment");
        }
    }
}


