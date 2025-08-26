using admin_application.Commands;
using admin_application.Handlers.Implementations.Environments;
using admin_application.Interfaces;

using admin_domain.Entities;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.Environments;

public class UpdateEnvironmentCommandHandlerTests
{
	[Fact]
	public async Task HandleAsync_RepoSucceeds_ReturnsUpdatedEnvironment()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IEnvironmentRepository>();
		repo.Setup(r => r.UpdateAsync(It.IsAny<admin_domain.Entities.Environment>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((admin_domain.Entities.Environment e, CancellationToken _) => Result.Ok(e));

		var handler = new UpdateEnvironmentCommandHandler(repo.Object);
		var cmd = new UpdateEnvironmentCommand { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = fixture.Create<string>() };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(cmd.Id, result.Value.Id);
		Assert.Equal(cmd.ProjectId, result.Value.ProjectId);
		repo.Verify(r => r.UpdateAsync(It.IsAny<admin_domain.Entities.Environment>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HandleAsync_RepoFails_ReturnsFailure()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IEnvironmentRepository>();
		repo.Setup(r => r.UpdateAsync(It.IsAny<admin_domain.Entities.Environment>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Fail<admin_domain.Entities.Environment>("NotFound"));

		var handler = new UpdateEnvironmentCommandHandler(repo.Object);
		var cmd = new UpdateEnvironmentCommand { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = fixture.Create<string>() };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailed);
		repo.Verify(r => r.UpdateAsync(It.IsAny<admin_domain.Entities.Environment>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}