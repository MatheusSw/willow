using System;
using System.Threading.Tasks;

using admin_domain.Entities;

using admin_infrastructure.Db;
using admin_infrastructure.Repositories.Organizations;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace admin_infrastructure_tests.Repositories.Organizations;

public class OrganizationRepositoryTests
{
	[Fact]
	public async Task CreateAsync_SavesOrganization_ReturnsOk()
	{
		// Arrange
		await using var db = InfrastructureFixtures.CreateInMemoryDb();
		var repo = new OrganizationRepository(db);
		var model = new Organization { Id = Guid.NewGuid(), Name = "org" };

		// Act
		var result = await repo.CreateAsync(model, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		var saved = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == model.Id);
		Assert.NotNull(saved);
		Assert.Equal(model.Name, saved.Name);
	}
}