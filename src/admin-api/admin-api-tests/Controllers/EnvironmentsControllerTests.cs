using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Handlers.Interfaces.Environments;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace admin_api_tests.Controllers;

public class EnvironmentsControllerTests
{
    [Fact]
    public async Task List_NoItems_Returns204()
    {
        var create = new Mock<ICreateEnvironmentCommandHandler>();
        var update = new Mock<IUpdateEnvironmentCommandHandler>();
        var delete = new Mock<IDeleteEnvironmentCommandHandler>();
        var getById = new Mock<IGetEnvironmentByIdQueryHandler>();
        var list = new Mock<IListEnvironmentsQueryHandler>();
        list.Setup(h => h.HandleAsync(It.IsAny<ListEnvironmentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<admin_domain.Entities.Environment>()));

        var controller = new EnvironmentsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task List_WithItems_Returns200WithResponses()
    {
        var create = new Mock<ICreateEnvironmentCommandHandler>();
        var update = new Mock<IUpdateEnvironmentCommandHandler>();
        var delete = new Mock<IDeleteEnvironmentCommandHandler>();
        var getById = new Mock<IGetEnvironmentByIdQueryHandler>();
        var list = new Mock<IListEnvironmentsQueryHandler>();
        var item = new admin_domain.Entities.Environment { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Key = "key" };
        list.Setup(h => h.HandleAsync(It.IsAny<ListEnvironmentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<admin_domain.Entities.Environment> { item }));

        var controller = new EnvironmentsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<List<EnvironmentResponse>>(ok.Value);
        Assert.Single(payload);
        Assert.Equal(item.Id, payload[0].Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var create = new Mock<ICreateEnvironmentCommandHandler>();
        var update = new Mock<IUpdateEnvironmentCommandHandler>();
        var delete = new Mock<IDeleteEnvironmentCommandHandler>();
        var getById = new Mock<IGetEnvironmentByIdQueryHandler>();
        getById.Setup(h => h.HandleAsync(It.IsAny<GetEnvironmentByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<admin_domain.Entities.Environment>("NotFound"));
        var list = new Mock<IListEnvironmentsQueryHandler>();

        var controller = new EnvironmentsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Failed_ReturnsProblem500()
    {
        var create = new Mock<ICreateEnvironmentCommandHandler>();
        create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateEnvironmentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<admin_domain.Entities.Environment>("error"));
        var update = new Mock<IUpdateEnvironmentCommandHandler>();
        var delete = new Mock<IDeleteEnvironmentCommandHandler>();
        var getById = new Mock<IGetEnvironmentByIdQueryHandler>();
        var list = new Mock<IListEnvironmentsQueryHandler>();

        var controller = new EnvironmentsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var req = new CreateEnvironmentRequest { ProjectId = Guid.NewGuid(), Key = "key" };
        var action = await controller.Create(req, CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
    }
}