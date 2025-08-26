using admin_application.Commands;
using admin_application.Handlers.Implementations.Organizations;
using admin_application.Interfaces;

using admin_domain.Entities;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentResults;

using Moq;

namespace admin_application_tests.Handlers.Implementations.Organizations;

public class CreateOrganizationCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoSucceeds_ReturnsCreatedOrganization()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IOrganizationRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization o, CancellationToken _) => Result.Ok(o));

        var handler = new CreateOrganizationCommandHandler(repo.Object);
        var cmd = new CreateOrganizationCommand { Name = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cmd.Name, result.Value.Name);
        repo.Verify(r => r.CreateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}