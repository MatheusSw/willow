using admin_application.Commands;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Handlers.Interfaces;
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
		var featureStateRepo = new Mock<IFeatureStateRepository>();
		var featureRepo = new Mock<IFeatureRepository>();
		var environmentRepo = new Mock<IEnvironmentRepository>();
		var publishHandler = new Mock<IPublishFeatureStateUpdatedCommandHandler>();

		var feature = new Feature { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "test-feature" };
		var environment = new admin_domain.Entities.Environment { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = "test-env" };

		featureStateRepo.Setup(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((FeatureState fs, CancellationToken _) => Result.Ok(fs));
		featureRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(feature));
		environmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(environment));
		publishHandler.Setup(p => p.HandleAsync(It.IsAny<PublishFeatureStateUpdatedCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok());

		var handler = new UpdateFeatureStateCommandHandler(featureStateRepo.Object, featureRepo.Object, environmentRepo.Object, publishHandler.Object);
		var cmd = new UpdateFeatureStateCommand { Id = Guid.NewGuid(), FeatureId = feature.Id, EnvironmentId = environment.Id, Enabled = true, Reason = "r" };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(cmd.Id, result.Value.Id);
		featureStateRepo.Verify(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()), Times.Once);
		publishHandler.Verify(p => p.HandleAsync(It.IsAny<PublishFeatureStateUpdatedCommand>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HandleAsync_RepoFails_ReturnsFailure()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var featureStateRepo = new Mock<IFeatureStateRepository>();
		var featureRepo = new Mock<IFeatureRepository>();
		var environmentRepo = new Mock<IEnvironmentRepository>();
		var publishHandler = new Mock<IPublishFeatureStateUpdatedCommandHandler>();

		var feature = new Feature { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "test-feature" };
		var environment = new admin_domain.Entities.Environment { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = "test-env" };

		featureStateRepo.Setup(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Fail<FeatureState>("NotFound"));
		featureRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(feature));
		environmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(environment));

		var handler = new UpdateFeatureStateCommandHandler(featureStateRepo.Object, featureRepo.Object, environmentRepo.Object, publishHandler.Object);
		var cmd = new UpdateFeatureStateCommand { Id = Guid.NewGuid(), FeatureId = feature.Id, EnvironmentId = environment.Id, Enabled = false, Reason = string.Empty };

		// Act
		var result = await handler.HandleAsync(cmd, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailed);
		featureStateRepo.Verify(r => r.UpdateAsync(It.IsAny<FeatureState>(), It.IsAny<CancellationToken>()), Times.Once);
		// Event publishing and cache invalidation should not be called when update fails
		publishHandler.Verify(p => p.HandleAsync(It.IsAny<PublishFeatureStateUpdatedCommand>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}