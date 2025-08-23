using admin_application.Commands;
using admin_application.Handlers.Implementations.Projects;
using admin_application.Interfaces;

using admin_domain.Entities;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.Projects;

public class CreateProjectCommandHandlerTests
{
	[Fact]
	public async Task HandleAsync_RepoSucceeds_ReturnsCreatedProject()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IProjectRepository>();
		repo.Setup(r => r.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Project p, CancellationToken _) => Result.Ok(p));

		var handler = new CreateProjectCommandHandler(repo.Object);
		var cmd = new CreateProjectCommand { OrgId = Guid.NewGuid(), Name = fixture.Create<string>() };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(cmd.OrgId, result.Value.OrgId);
		repo.Verify(r => r.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}