using System;

using admin_infrastructure.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace admin_infrastructure_tests;

public static class InfrastructureFixtures
{
	public static FeatureToggleDbContext CreateInMemoryDb()
	{
		var options = new DbContextOptionsBuilder<FeatureToggleDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FeatureToggleDbContext(options);
	}

	public static IDistributedCache CreateMemoryDistributedCache()
	{
		return new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
	}
}