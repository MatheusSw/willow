using System;
using System.Threading.Tasks;
using evaluation_infrastructure.Db;
using evaluation_infrastructure.Db.Entities;
using evaluation_infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace evaluation_infrastructure_tests;

public class InfrastructureFixtures
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

public class ApiKeyRepositoryTests
{
	[Fact]
	public async Task ValidateAsync_CachesValidKey_ReturnsTrueOnSecondCall()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var cache = InfrastructureFixtures.CreateMemoryDistributedCache();
		var repo = new ApiKeyRepository(db, cache);

		var keyHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData("testkey"u8.ToArray()));
		db.ApiKeys.Add(new ApiKey
		{
			Id = Guid.NewGuid(),
			OrgId = Guid.NewGuid(),
			ProjectId = null,
			Role = "reader",
			Scopes = Array.Empty<string>(),
			HashedKey = keyHash,
			Active = true
		});
		await db.SaveChangesAsync();

		// Act
		var first = await repo.ValidateAsync("testkey", default);
		var second = await repo.ValidateAsync("testkey", default);

		// Assert
		Assert.True(first);
		Assert.True(second);
	}

	[Fact]
	public async Task ValidateAsync_KeyNotFound_ReturnsFalse()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var cache = InfrastructureFixtures.CreateMemoryDistributedCache();
		var repo = new ApiKeyRepository(db, cache);

		// Act
		var result = await repo.ValidateAsync("missing", default);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task ValidateAsync_InactiveKey_ReturnsFalse()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var cache = InfrastructureFixtures.CreateMemoryDistributedCache();
		var repo = new ApiKeyRepository(db, cache);

		var keyHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData("inactive"u8.ToArray()));
		db.ApiKeys.Add(new ApiKey
		{
			Id = Guid.NewGuid(),
			OrgId = Guid.NewGuid(),
			ProjectId = null,
			Role = "reader",
			Scopes = Array.Empty<string>(),
			HashedKey = keyHash,
			Active = false
		});
		await db.SaveChangesAsync();

		// Act
		var result = await repo.ValidateAsync("inactive", default);

		// Assert
		Assert.False(result);
	}
}
