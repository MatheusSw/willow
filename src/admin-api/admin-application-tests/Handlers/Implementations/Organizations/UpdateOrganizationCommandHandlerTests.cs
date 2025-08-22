using AutoFixture;
using AutoFixture.AutoMoq;
using admin_application.Commands;
using admin_application.Handlers.Implementations.Organizations;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Moq;

namespace admin_application_tests.Handlers.Implementations.Organizations;

public class UpdateOrganizationCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepoSucceeds_ReturnsUpdatedOrganization()
    {
        // Arrange
        var fixture = FixtureFactory.Create();
        var repo = new Mock<IOrganizationRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization o, CancellationToken _) => Result.Ok(o));

        var handler = new UpdateOrganizationCommandHandler(repo.Object);
        var cmd = new UpdateOrganizationCommand { Id = Guid.NewGuid(), Name = fixture.Create<string>() };

        // Act
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cmd.Id, result.Value.Id);
        Assert.Equal(cmd.Name, result.Value.Name);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


