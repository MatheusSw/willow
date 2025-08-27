using admin_application.Commands;
using admin_application.Events;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Interfaces;
using admin_domain;
using Moq;

namespace admin_application_tests.Handlers.Implementations.FeatureStates;

public class PublishFeatureStateUpdatedCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_SuccessfulPublishingAndCacheInvalidation_ReturnsSuccess()
    {
        // Arrange
        FixtureFactory.Create();
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<FeatureStateUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = Guid.NewGuid(),
            FeatureName = "new-feature",
            Enabled = true
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        eventPublisher.Verify(p => p.PublishAsync("ft:updates", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheInvalidator.Verify(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_EventPublishingFails_StillReturnsSuccessAndInvalidatesCache()
    {
        // Arrange
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<FeatureStateUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Redis connection failed"));
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = Guid.NewGuid(),
            FeatureName = "test-feature",
            Enabled = false
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess); // Should not fail the operation
        eventPublisher.Verify(p => p.PublishAsync("ft:updates", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheInvalidator.Verify(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CacheInvalidationFails_StillReturnsSuccessAndPublishesEvent()
    {
        // Arrange
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<FeatureStateUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("Cache timeout"));

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = Guid.NewGuid(),
            FeatureName = "cache-feature",
            Enabled = true
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess); // Should not fail the operation
        eventPublisher.Verify(p => p.PublishAsync("ft:updates", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheInvalidator.Verify(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_BothOperationsFail_StillReturnsSuccess()
    {
        // Arrange
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<FeatureStateUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Event publishing failed"));
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache invalidation failed"));

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = Guid.NewGuid(),
            FeatureName = "failing-feature",
            Enabled = false
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess); // Should not fail the operation even when both operations fail
        eventPublisher.Verify(p => p.PublishAsync("ft:updates", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheInvalidator.Verify(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PublishesCorrectEventData()
    {
        // Arrange
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<FeatureStateUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var projectId = Guid.NewGuid();
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = projectId,
            FeatureName = "test-feature",
            Enabled = true
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        eventPublisher.Verify(p => p.PublishAsync<FeatureStateUpdatedEvent>("ft:updates",
            It.Is<FeatureStateUpdatedEvent>(e => e.ProjectId == command.ProjectId && e.Feature == command.FeatureName && e.Enabled == command.Enabled),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_InvalidatesCorrectCacheKey()
    {
        // Arrange
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        cacheInvalidator.Setup(c => c.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new PublishFeatureStateUpdatedCommandHandler(eventPublisher.Object, cacheInvalidator.Object);
        var projectId = Guid.NewGuid();
        var command = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = projectId,
            FeatureName = "cache-test-feature",
            Enabled = false
        };

        var expectedCacheKey = CacheKeys.FeatureConfig(command.ProjectId, command.FeatureName);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        cacheInvalidator.Verify(c => c.InvalidateAsync(expectedCacheKey, It.IsAny<CancellationToken>()), Times.Once);
    }
}
