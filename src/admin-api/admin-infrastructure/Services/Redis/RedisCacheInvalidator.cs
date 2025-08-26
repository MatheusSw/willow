using admin_application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace admin_infrastructure.Services.Redis;

public sealed class RedisCacheInvalidator : ICacheInvalidator
{
	private readonly IDistributedCache _distributedCache;

	public RedisCacheInvalidator(IDistributedCache distributedCache)
	{
		_distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
	}

	public async Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
		}

		var log = Log.ForContext<RedisCacheInvalidator>()
			.ForContext("CacheKey", key);

		try
		{
			log.Information("Invalidating cache key");
			await _distributedCache.RemoveAsync(key, cancellationToken);
			log.Information("Cache key invalidated successfully");
		}
		catch (Exception ex)
		{
			log.Error(ex, "Failed to invalidate cache key");
			throw;
		}
	}
}
