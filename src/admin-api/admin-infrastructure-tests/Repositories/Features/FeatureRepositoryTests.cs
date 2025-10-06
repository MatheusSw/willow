using System;
using System.Threading.Tasks;

using admin_domain.Entities;

using admin_infrastructure.Db;
using admin_infrastructure.Repositories.Features;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace admin_infrastructure_tests.Repositories.Features;

public class FeatureRepositoryTests
{
	[Fact]
	public async Task CreateAsync_SavesFeature_ReturnsOk()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var repo = new FeatureRepository(db);
		var model = new Feature { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "f", Description = "d" };

		// Act
		var result = await repo.CreateAsync(model, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		var saved = await db.Features.AsNoTracking().FirstOrDefaultAsync(f => f.Id == model.Id);
		Assert.NotNull(saved);
		Assert.Equal(model.Name, saved.Name);
	}
}