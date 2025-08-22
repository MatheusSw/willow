using AutoFixture;
using AutoFixture.AutoMoq;
using admin_application.Handlers.Implementations.FeatureStates;
using admin_application.Interfaces;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Moq;

namespace admin_application_tests.Handlers.Implementations.FeatureStates;

public class ListFeatureStatesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoReturnsList_ReturnsList()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IFeatureStateRepository>();
        var item = new FeatureState { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = true };
        repo.Setup(r => r.ListAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<FeatureState> { item }));

        var handler = new ListFeatureStatesQueryHandler(repo.Object);
        var query = new ListFeatureStatesQuery { FeatureId = item.FeatureId, EnvironmentId = item.EnvironmentId };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        repo.Verify(r => r.ListAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


