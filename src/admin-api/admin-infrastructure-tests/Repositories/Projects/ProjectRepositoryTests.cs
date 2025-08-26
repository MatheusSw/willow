using System;
using System.Threading.Tasks;

using admin_domain.Entities;

using admin_infrastructure.Db;
using admin_infrastructure.Repositories.Projects;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace admin_infrastructure_tests.Repositories.Projects;

public class ProjectRepositoryTests
{
	[Fact]
	public async Task CreateAsync_SavesProject_ReturnsOk()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var repo = new ProjectRepository(db);
		var model = new Project { Id = Guid.NewGuid(), OrgId = Guid.NewGuid(), Name = "p" };

		// Act
		var result = await repo.CreateAsync(model, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		var saved = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.Id);
		Assert.NotNull(saved);
		Assert.Equal(model.Name, saved.Name);
	}
}