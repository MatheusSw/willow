using System.Security.Cryptography;
using System.Text;
using evaluation_application.Interfaces;
using evaluation_infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace evaluation_infrastructure.Repositories;

/// <summary>
/// Repository responsible for validating API keys using caching and database lookup.
/// </summary>
public sealed class ApiKeyRepository : IApiKeyRepository
{
    private readonly FeatureToggleDbContext _dbContext;
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyRepository"/>.
    /// </summary>
    /// <param name="dbContext">Feature toggle DbContext.</param>
    /// <param name="cache">Distributed cache used for API key caching.</param>
    public ApiKeyRepository(FeatureToggleDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    /// <summary>
    /// Validates an API key. Uses a short-lived cache for positive results.
    /// </summary>
    /// <param name="apiKey">Raw API key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if valid; otherwise false.</returns>
    public async Task<bool> ValidateAsync(string apiKey, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<ApiKeyRepository>();
        log.Information("Validate API key started");

        var hash = ComputeSha256Base64(apiKey);
        var cacheKey = Utilities.CacheKeys.ApiKey(hash);
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached == "1")
        {
            log.Information("API key cache hit");
            return true;
        }

        var exists = await _dbContext.ApiKeys.AsNoTracking()
            .AnyAsync(k => k.HashedKey == hash && k.Active, cancellationToken);

        if (exists)
        {
            log.Information("API key valid, caching");
            
            await _cache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);
        }

        log.Information("Validate API key completed: {Valid}", exists);
        
        return exists;
    }

    /// <summary>
    /// Computes a base64-encoded SHA-256 hash for the provided input.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns>Base64-encoded SHA-256 hash.</returns>
    private static string ComputeSha256Base64(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}


