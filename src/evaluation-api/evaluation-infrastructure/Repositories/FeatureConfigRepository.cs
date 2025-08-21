using System.Text.Json;
using evaluation_application.Interfaces;
using evaluation_domain.Rules;
using evaluation_infrastructure.Db;
using evaluation_infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace evaluation_infrastructure.Repositories;

public sealed class FeatureConfigRepository : IFeatureConfigRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly FeatureToggleDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public FeatureConfigRepository(FeatureToggleDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<(bool Found, FeatureConfig? Config)> TryGetConfigAsync(string project, string environment, string feature, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<FeatureConfigRepository>()
            .ForContext("project", project)
            .ForContext("environment", environment)
            .ForContext("feature", feature);
        log.Information("TryGetConfig started");
        // Resolve identifiers
        var projectEntity = await _dbContext.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Name == project, cancellationToken);
        if (projectEntity is null)
        {
            log.Information("Project not found");

            return (false, null);
        }

        var environmentEntity = await _dbContext.Environments.AsNoTracking()
            .FirstOrDefaultAsync(e => e.ProjectId == projectEntity.Id && e.Key == environment, cancellationToken);
        if (environmentEntity is null)
        {
            log.Information("Environment not found");

            return (false, null);
        }

        var featureEntity = await _dbContext.Features.AsNoTracking()
            .FirstOrDefaultAsync(f => f.ProjectId == projectEntity.Id && f.Name == feature, cancellationToken);
        if (featureEntity is null)
        {
            log.Information("Feature not found");

            return (false, null);
        }

        var cacheKey = Utilities.CacheKeys.FeatureConfig(projectEntity.Id, environment, feature);
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            var config = JsonSerializer.Deserialize<FeatureConfig>(cached, JsonOptions);
            if (config is not null)
            {
                log.Information("Cache hit");
                return (true, config);
            }
        }

        log.Information("Cache miss, querying database");

        // DB fallback
        var configToCache = await LoadFromDatabaseAsync(featureEntity, environmentEntity, cancellationToken);
        if (configToCache is null)
        {
            log.Information("Feature state not found");
            return (false, null);
        }

        var payload = JsonSerializer.Serialize(configToCache, JsonOptions);
        await _cache.SetStringAsync(cacheKey, payload, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        }, cancellationToken);

        log.Information("TryGetConfig completed");

        return (true, configToCache);
    }

    private async Task<FeatureConfig?> LoadFromDatabaseAsync(Feature featureEntity, EnvironmentEntity environmentEntity, CancellationToken cancellationToken)
    {
        var featureState = await _dbContext.FeatureStates.AsNoTracking()
            .FirstOrDefaultAsync(fs => fs.FeatureId == featureEntity.Id && fs.EnvironmentId == environmentEntity.Id, cancellationToken);

        if (featureState is null)
        {
            return null;
        }

        var rules = await _dbContext.Rules.AsNoTracking()
            .Where(r => r.FeatureId == featureEntity.Id && (r.EnvironmentId == environmentEntity.Id || r.EnvironmentId == null))
            .OrderBy(r => r.Priority)
            .ToListAsync(cancellationToken);

        var mappedRules = rules.Select(r => new FeatureRule
        {
            MatchType = r.MatchType,
            Priority = r.Priority,
            Conditions = JsonSerializer.Deserialize<List<RuleCondition>>(r.ConditionsJson, JsonOptions) ?? new List<RuleCondition>()
        }).ToList();

        var configToCache = new FeatureConfig
        {
            Enabled = featureState.Enabled,
            Reason = featureState.Reason,
            Rules = mappedRules,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        return configToCache;
    }
}


