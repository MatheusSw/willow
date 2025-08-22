using System;
using System.Threading.Tasks;
using admin_infrastructure.Repositories.Environments;
using admin_infrastructure.Db;
using admin_domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace admin_infrastructure_tests.Repositories.Environments;

public class EnvironmentRepositoryTests
{
    [Fact]
    public async Task CreateAsync_SavesEnvironment_ReturnsOk()
    {
        // Arrange
        await using var db = InfrastructureFixtures.CreateInMemoryDb();
        var repo = new EnvironmentRepository(db);
        var model = new admin_domain.Entities.Environment { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = "k" };

        // Act
        var result = await repo.CreateAsync(model, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var saved = await db.Environments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.Id);
        Assert.NotNull(saved);
        Assert.Equal(model.Key, saved.Key);
    }
}


