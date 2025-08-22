using AutoFixture;
using AutoFixture.AutoMoq;
using admin_application.Handlers.Implementations.Rules;
using admin_application.Interfaces;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Moq;

namespace admin_application_tests.Handlers.Implementations.Rules;

public class ListRulesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoReturnsList_ReturnsList()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IRuleRepository>();
        var item = new Rule { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1 };
        repo.Setup(r => r.ListAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Rule> { item }));

        var handler = new ListRulesQueryHandler(repo.Object);
        var query = new ListRulesQuery { FeatureId = item.FeatureId, EnvironmentId = item.EnvironmentId };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        repo.Verify(r => r.ListAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


