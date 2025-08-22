using AutoFixture;
using AutoFixture.AutoMoq;
using evaluation_application.Handlers;
using evaluation_application.Interfaces;
using evaluation_application.Queries;
using Moq;

namespace evaluation_application_tests;

public class EvaluateFeatureQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ConfigFound_EvaluatesAndReturnsResult()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var repo = new Mock<IFeatureConfigRepository>();
        repo.Setup(r => r.TryGetConfigAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, new FeatureConfig { Enabled = true, Reason = "default:true" }));
        var handler = new EvaluateFeatureQueryHandler(repo.Object);

        var query = new EvaluateFeatureQuery
        {
            Project = fixture.Create<string>(),
            Feature = fixture.Create<string>(),
            Attributes = new Dictionary<string, string> { { "orgId", "acme" } }
        };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Enabled);
        Assert.NotNull(result.Reason);
    }

    [Fact]
    public async Task HandleAsync_ConfigNotFound_ReturnsNotFoundReason()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var repo = new Mock<IFeatureConfigRepository>();
        repo.Setup(r => r.TryGetConfigAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (FeatureConfig?)null));
        var handler = new EvaluateFeatureQueryHandler(repo.Object);

        var query = new EvaluateFeatureQuery
        {
            Project = fixture.Create<string>(),
            Environment = fixture.Create<string>(),
            Feature = fixture.Create<string>(),
            Attributes = new Dictionary<string, string>()
        };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Enabled);
        Assert.Equal("not_found", result.Reason);
        Assert.Null(result.Variant);
    }
}