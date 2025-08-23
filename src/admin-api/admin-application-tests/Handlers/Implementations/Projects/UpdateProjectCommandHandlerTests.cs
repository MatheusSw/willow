using admin_application.Commands;
using admin_application.Handlers.Implementations.Projects;
using admin_application.Interfaces;

using admin_domain.Entities;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.Projects;

public class UpdateProjectCommandHandlerTests
{
	[Fact]
	public async Task HandleAsync_RepoSucceeds_ReturnsUpdatedProject()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IProjectRepository>();
		repo.Setup(r => r.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Project p, CancellationToken _) => Result.Ok(p));

		var handler = new UpdateProjectCommandHandler(repo.Object);
		var cmd = new UpdateProjectCommand { Id = Guid.NewGuid(), OrgId = Guid.NewGuid(), Name = fixture.Create<string>() };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(cmd.Id, result.Value.Id);
		repo.Verify(r => r.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}