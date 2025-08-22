using System.Text.Json;
using evaluation_application.Interfaces;
using evaluation_domain.Rules;
using evaluation_infrastructure.Db;
using evaluation_infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace evaluation_infrastructure.Repositories;

/// <summary>
/// Repository that loads feature configurations from cache or database and maps them to application models.
/// </summary>
public sealed class FeatureConfigRepository : IFeatureConfigRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly FeatureToggleDbContext _dbContext;
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of <see cref="FeatureConfigRepository"/>.
    /// </summary>
    /// <param name="dbContext">Feature toggle DbContext.</param>
    /// <param name="cache">Distributed cache for storing feature configs.</param>
    public FeatureConfigRepository(FeatureToggleDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    /// <summary>
    /// Attempts to resolve a feature configuration for a given project/environment/feature.
    /// </summary>
    /// <param name="project">Project name.</param>
    /// <param name="environment">Environment key.</param>
    /// <param name="feature">Feature name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple indicating if found and the configuration if available.</returns>
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

    /// <summary>
    /// Loads feature configuration from the database when not present in cache.
    /// </summary>
    /// <param name="featureEntity">Feature entity.</param>
    /// <param name="environmentEntity">Environment entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Feature configuration or null when no state exists.</returns>
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


