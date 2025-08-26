using admin_application.Commands;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Interfaces;

using admin_domain.Entities;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.FeatureStates;

public class UpdateFeatureStateCommandHandlerTests
{
	[Fact]
	public async Task HandleAsync_RepoSucceeds_ReturnsUpdatedFeatureState()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IFeatureStateRepository>();
		repo.Setup(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((FeatureState fs, CancellationToken _) => Result.Ok(fs));

		var handler = new UpdateFeatureStateCommandHandler(repo.Object);
		var cmd = new UpdateFeatureStateCommand { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = true, Reason = "r" };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(cmd.Id, result.Value.Id);
		repo.Verify(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HandleAsync_RepoFails_ReturnsFailure()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var repo = new Mock<IFeatureStateRepository>();
		repo.Setup(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Fail<FeatureState>("NotFound"));

		var handler = new UpdateFeatureStateCommandHandler(repo.Object);
		var cmd = new UpdateFeatureStateCommand { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = false, Reason = null };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailed);
		repo.Verify(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}