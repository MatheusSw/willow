using admin_application.Commands;
using admin_application.Handlers.Implementations.Rules;
using admin_application.Interfaces;

using admin_domain.Entities;
using admin_domain.Rules;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.Rules;

public class CreateRuleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoSucceeds_ReturnsCreatedRule()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IRuleRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rule r, CancellationToken _) => Result.Ok(r));

        var handler = new CreateRuleCommandHandler(repo.Object);
        var cmd = new CreateRuleCommand { FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1, MatchType = "all", Conditions = [] };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cmd.FeatureId, result.Value.FeatureId);
        repo.Verify(r => r.CreateAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RepoFails_ReturnsFailure()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IRuleRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Rule>("error"));

        var handler = new CreateRuleCommandHandler(repo.Object);
        var cmd = new CreateRuleCommand { FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1, MatchType = "any", Conditions = [] };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        repo.Verify(r => r.CreateAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}