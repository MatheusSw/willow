using System.Security.Cryptography;
using System.Text;

using admin_application.Interfaces;

using admin_infrastructure.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using Serilog;

namespace admin_infrastructure.Repositories.Security;

public sealed class ApiKeyRepository(FeatureToggleDbContext dbContext, IDistributedCache cache) : IApiKeyRepository
{
	public async Task<bool> ValidateAsync(string apiKey, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<ApiKeyRepository>();
		log.Information("Validate API key started");

		var hash = ComputeSha256Base64(apiKey);
		var cacheKey = Utilities.CacheKeys.ApiKey(hash);
		var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
		if (cached == "1")
		{
			log.Information("API key cache hit");
			return true;
		}

		var exists = await dbContext.ApiKeys.AsNoTracking()
			.AnyAsync(k => k.HashedKey == hash && k.Active, cancellationToken);

		if (exists)
		{
			log.Information("API key valid, caching");

			await cache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
			}, cancellationToken);
		}

		log.Information("Validate API key completed: {Valid}", exists);

		return exists;
	}

	private static string ComputeSha256Base64(string input)
	{
		var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
		return Convert.ToBase64String(bytes);
	}
}