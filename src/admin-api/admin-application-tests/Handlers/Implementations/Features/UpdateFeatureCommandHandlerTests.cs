using AutoFixture;
using AutoFixture.AutoMoq;
using admin_application.Commands;
using admin_application.Handlers.Implementations.Features;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Moq;

namespace admin_application_tests.Handlers.Implementations.Features;

public class UpdateFeatureCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoSucceeds_ReturnsUpdatedFeature()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IFeatureRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feature f, CancellationToken _) => Result.Ok(f));

        var handler = new UpdateFeatureCommandHandler(repo.Object);
        var cmd = new UpdateFeatureCommand { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = fixture.Create<string>(), Description = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cmd.Id, result.Value.Id);
        Assert.Equal(cmd.ProjectId, result.Value.ProjectId);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RepoFails_ReturnsFailure()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IFeatureRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Feature>("NotFound"));

        var handler = new UpdateFeatureCommandHandler(repo.Object);
        var cmd = new UpdateFeatureCommand { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = fixture.Create<string>(), Description = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


