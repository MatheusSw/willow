using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Handlers.Interfaces.Organizations;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace admin_api_tests.Controllers;

public class OrganizationsControllerTests
{
    [Fact]
    public async Task List_NoItems_Returns204()
    {
        var create = new Mock<ICreateOrganizationCommandHandler>();
        var update = new Mock<IUpdateOrganizationCommandHandler>();
        var delete = new Mock<IDeleteOrganizationCommandHandler>();
        var getById = new Mock<IGetOrganizationByIdQueryHandler>();
        var list = new Mock<IListOrganizationsQueryHandler>();
        list.Setup(h => h.HandleAsync(It.IsAny<ListOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Organization>()));

        var controller = new OrganizationsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task List_WithItems_Returns200WithResponses()
    {
        var create = new Mock<ICreateOrganizationCommandHandler>();
        var update = new Mock<IUpdateOrganizationCommandHandler>();
        var delete = new Mock<IDeleteOrganizationCommandHandler>();
        var getById = new Mock<IGetOrganizationByIdQueryHandler>();
        var list = new Mock<IListOrganizationsQueryHandler>();
        var item = new Organization { Id = Guid.NewGuid(), Name = "org" };
        list.Setup(h => h.HandleAsync(It.IsAny<ListOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Organization> { item }));

        var controller = new OrganizationsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<List<OrganizationResponse>>(ok.Value);
        Assert.Single(payload);
        Assert.Equal(item.Id, payload[0].Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var create = new Mock<ICreateOrganizationCommandHandler>();
        var update = new Mock<IUpdateOrganizationCommandHandler>();
        var delete = new Mock<IDeleteOrganizationCommandHandler>();
        var getById = new Mock<IGetOrganizationByIdQueryHandler>();
        getById.Setup(h => h.HandleAsync(It.IsAny<GetOrganizationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Organization>("NotFound"));
        var list = new Mock<IListOrganizationsQueryHandler>();

        var controller = new OrganizationsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Failed_ReturnsProblem500()
    {
        var create = new Mock<ICreateOrganizationCommandHandler>();
        create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateOrganizationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Organization>("error"));
        var update = new Mock<IUpdateOrganizationCommandHandler>();
        var delete = new Mock<IDeleteOrganizationCommandHandler>();
        var getById = new Mock<IGetOrganizationByIdQueryHandler>();
        var list = new Mock<IListOrganizationsQueryHandler>();

        var controller = new OrganizationsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var action = await controller.Create(new CreateOrganizationRequest { Name = "org" }, CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
    }
}