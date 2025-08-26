using admin_application.Commands;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Interfaces;
using Moq;

namespace admin_application_tests.Handlers.Implementations.FeatureStates;

public class PublishFeatureStateUpdatedCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_SuccessfulPublishingAndCacheInvalidation_ReturnsSuccess()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
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
        var fixture = FixtureFactory.Create();
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
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
        var fixture = FixtureFactory.Create();
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
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
        var fixture = FixtureFactory.Create();
        var eventPublisher = new Mock<IEventPublisher>();
        var cacheInvalidator = new Mock<ICacheInvalidator>();

        eventPublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
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
        var fixture = FixtureFactory.Create();
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
            FeatureName = "test-feature",
            Enabled = true
        };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        eventPublisher.Verify(p => p.PublishAsync("ft:updates",
            It.Is<object>(e =>
                e.GetType().GetProperty("ProjectId") != null && e.GetType().GetProperty("ProjectId").GetValue(e) != null && e.GetType().GetProperty("ProjectId").GetValue(e).Equals(projectId) &&
                e.GetType().GetProperty("Environment") != null && e.GetType().GetProperty("Environment").GetValue(e) != null && e.GetType().GetProperty("Environment").GetValue(e).Equals("staging") &&
                e.GetType().GetProperty("Feature") != null && e.GetType().GetProperty("Feature").GetValue(e) != null && e.GetType().GetProperty("Feature").GetValue(e).Equals("test-feature") &&
                e.GetType().GetProperty("Enabled") != null && e.GetType().GetProperty("Enabled").GetValue(e) != null && e.GetType().GetProperty("Enabled").GetValue(e).Equals(true)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_InvalidatesCorrectCacheKey()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
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

        var expectedCacheKey = $"ft:cfg:{projectId}:production:cache-test-feature";

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        cacheInvalidator.Verify(c => c.InvalidateAsync(expectedCacheKey, It.IsAny<CancellationToken>()), Times.Once);
    }
}
