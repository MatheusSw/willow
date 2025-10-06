using System;
using System.Threading.Tasks;

using admin_domain.Rules;

using admin_infrastructure.Db;
using admin_infrastructure.Repositories.Rules;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace admin_infrastructure_tests.Repositories.Rules;

public class RuleRepositoryTests
{
	[Fact]
	public async Task CreateAsync_SavesRule_ReturnsOk()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var repo = new RuleRepository(db);
		var model = new admin_domain.Entities.Rule { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1, MatchType = admin_domain.Rules.MatchType.All, Conditions = [] };

		// Act
		var result = await repo.CreateAsync(model, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		var saved = await db.Rules.AsNoTracking().FirstOrDefaultAsync(r => r.Id == model.Id);
		Assert.NotNull(saved);
		Assert.Equal(model.FeatureId, saved.FeatureId);
	}
}