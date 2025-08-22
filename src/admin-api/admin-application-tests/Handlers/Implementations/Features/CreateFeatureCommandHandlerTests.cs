using AutoFixture;
using AutoFixture.AutoMoq;
using admin_application.Commands;
using admin_application.Handlers.Implementations.Features;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Moq;

namespace admin_application_tests.Handlers.Implementations.Features;

public class CreateFeatureCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoSucceeds_ReturnsCreatedFeature()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IFeatureRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feature f, CancellationToken _) => Result.Ok(f));

        var handler = new CreateFeatureCommandHandler(repo.Object);
        var cmd = new CreateFeatureCommand { ProjectId = Guid.NewGuid(), Name = fixture.Create<string>(), Description = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cmd.ProjectId, result.Value.ProjectId);
        Assert.Equal(cmd.Name, result.Value.Name);
        repo.Verify(r => r.CreateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RepoFails_ReturnsFailure()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IFeatureRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Feature>("error"));

        var handler = new CreateFeatureCommandHandler(repo.Object);
        var cmd = new CreateFeatureCommand { ProjectId = Guid.NewGuid(), Name = fixture.Create<string>(), Description = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        repo.Verify(r => r.CreateAsync(It.IsAny<Feature>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


