using AutoFixture;
using AutoFixture.AutoMoq;
using evaluation_application.Handlers;
using evaluation_application.Interfaces;
using evaluation_application.Queries;
using Moq;

namespace evaluation_application_tests;

public class ValidateApiKeyQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_MissingApiKey_ReturnsFalseAndSkipsRepo()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var repo = new Mock<IApiKeyRepository>(MockBehavior.Strict);
        var handler = new ValidateApiKeyQueryHandler(repo.Object);

        var query = new ValidateApiKeyQuery { ApiKey = string.Empty };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
        repo.Verify(r => r.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_RepoReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var repo = new Mock<IApiKeyRepository>();
        repo.Setup(r => r.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new ValidateApiKeyQueryHandler(repo.Object);

        var query = new ValidateApiKeyQuery { ApiKey = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result);
        repo.Verify(r => r.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RepoReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var repo = new Mock<IApiKeyRepository>();
        repo.Setup(r => r.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var handler = new ValidateApiKeyQueryHandler(repo.Object);

        var query = new ValidateApiKeyQuery { ApiKey = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
        repo.Verify(r => r.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


