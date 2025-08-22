using admin_application.Interfaces;
using admin_domain.Entities;
using admin_infrastructure.Db;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace admin_infrastructure.Repositories.Rules;

public sealed class RuleRepository : IRuleRepository
{
    private readonly FeatureToggleDbContext _dbContext;

    public RuleRepository(FeatureToggleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Rule>> CreateAsync(Rule rule, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RuleRepository>()
            .ForContext("FeatureId", rule.FeatureId)
            .ForContext("EnvironmentId", rule.EnvironmentId)
            .ForContext("Priority", rule.Priority)
            .ForContext("MatchType", rule.MatchType);

        log.Information("Rule Create started");

        try
        {
            var conditionsString = System.Text.Json.JsonSerializer.Serialize(rule.Conditions);
            var matchTypeString = rule.MatchType.ToString().ToLowerInvariant();

            var entity = new Db.Entities.Rule
            {
                Id = rule.Id,
                FeatureId = rule.FeatureId,
                EnvironmentId = rule.EnvironmentId,
                Priority = rule.Priority,
                MatchType = matchTypeString,
                ConditionsJson = conditionsString
            };

            _dbContext.Rules.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            log.Information("Rule Create completed");
            return Result.Ok(rule);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Rule Create failed");
            return Result.Fail("Failed to create rule");
        }
    }

    public async Task<Result<Rule>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RuleRepository>()
            .ForContext("Id", id);

        log.Information("Rule GetById started");

        var entity = await _dbContext.Rules.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("Rule not found");
            return Result.Fail("NotFound");
        }

        var model = new Rule
        {
            Id = entity.Id,
            FeatureId = entity.FeatureId,
            EnvironmentId = entity.EnvironmentId,
            Priority = entity.Priority,
            MatchType = entity.MatchType == "any" ? admin_domain.Rules.MatchType.Any : admin_domain.Rules.MatchType.All,
            Conditions = System.Text.Json.JsonSerializer.Deserialize<List<admin_domain.Rules.RuleCondition>>(entity.ConditionsJson) ?? new()
        };

        log.Information("Rule GetById completed");
        return Result.Ok(model);
    }

    public async Task<Result<List<Rule>>> ListAsync(Guid? featureId, Guid? environmentId, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RuleRepository>()
            .ForContext("FeatureId", featureId)
            .ForContext("EnvironmentId", environmentId);

        log.Information("Rule List started");

        var query = _dbContext.Rules.AsNoTracking().AsQueryable();
        if (featureId.HasValue)
        {
            query = query.Where(r => r.FeatureId == featureId.Value);
        }
        if (environmentId.HasValue)
        {
            query = query.Where(r => r.EnvironmentId == environmentId.Value);
        }

        var rows = await query
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.Id)
            .Select(r => new { r.Id, r.FeatureId, r.EnvironmentId, r.Priority, r.MatchType, r.ConditionsJson })
            .ToListAsync(cancellationToken);

        var result = rows.Select(r => new Rule
        {
            Id = r.Id,
            FeatureId = r.FeatureId,
            EnvironmentId = r.EnvironmentId,
            Priority = r.Priority,
            MatchType = r.MatchType == "any" ? admin_domain.Rules.MatchType.Any : admin_domain.Rules.MatchType.All,
            Conditions = System.Text.Json.JsonSerializer.Deserialize<List<admin_domain.Rules.RuleCondition>>(r.ConditionsJson) ?? new()
        }).ToList();

        log.Information("Rule List completed: Count={Count}", result.Count);
        return Result.Ok(result);
    }

    public async Task<Result<Rule>> UpdateAsync(Rule rule, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RuleRepository>()
            .ForContext("Id", rule.Id)
            .ForContext("FeatureId", rule.FeatureId)
            .ForContext("EnvironmentId", rule.EnvironmentId)
            .ForContext("Priority", rule.Priority)
            .ForContext("MatchType", rule.MatchType);

        log.Information("Rule Update started");

        try
        {
            var matchTypeString = rule.MatchType.ToString().ToLowerInvariant();
            var conditionsString = System.Text.Json.JsonSerializer.Serialize(rule.Conditions);

            var affected = await _dbContext.Rules
                .Where(r => r.Id == rule.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(r => r.FeatureId, rule.FeatureId)
                    .SetProperty(r => r.EnvironmentId, rule.EnvironmentId)
                    .SetProperty(r => r.Priority, rule.Priority)
                    .SetProperty(r => r.MatchType, matchTypeString)
                    .SetProperty(r => r.ConditionsJson, conditionsString), cancellationToken);

            if (affected == 0)
            {
                log.Information("Rule to update not found");
                return Result.Fail("NotFound");
            }

            log.Information("Rule Update completed");
            return Result.Ok(rule);
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Rule Update failed");
            return Result.Fail("Failed to update rule");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<RuleRepository>()
            .ForContext("Id", id);

        log.Information("Rule Delete started");

        var entity = await _dbContext.Rules.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity == null)
        {
            log.Information("Rule to delete not found");
            return Result.Fail("NotFound");
        }

        _dbContext.Rules.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            log.Information("Rule Delete completed");
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            log.Error(ex, "Rule Delete failed");
            return Result.Fail("Failed to delete rule");
        }
    }
}


