using System;
using System.Threading.Tasks;
using admin_infrastructure.Repositories.FeatureStates;
using admin_infrastructure.Db;
using admin_domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace admin_infrastructure_tests.Repositories.FeatureStates;

public class FeatureStateRepositoryTests
{
    [Fact]
    public async Task CreateAsync_SavesFeatureState_ReturnsOk()
    {
        // Arrange
        await using var db = InfrastructureFixtures.CreateInMemoryDb();
        var repo = new FeatureStateRepository(db);
        var model = new FeatureState { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = true, Reason = "r" };

        // Act
        var result = await repo.CreateAsync(model, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var saved = await db.FeatureStates.AsNoTracking().FirstOrDefaultAsync(f => f.Id == model.Id);
        Assert.NotNull(saved);
        Assert.Equal(model.FeatureId, saved.FeatureId);
    }
}


