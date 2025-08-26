using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace admin_api_tests.Controllers;

public class FeatureStatesControllerTests
{
    [Fact]
    public async Task List_NoItems_Returns204()
    {
        var create = new Mock<ICreateFeatureStateCommandHandler>();
        var update = new Mock<IUpdateFeatureStateCommandHandler>();
        var delete = new Mock<IDeleteFeatureStateCommandHandler>();
        var getById = new Mock<IGetFeatureStateByIdQueryHandler>();
        var list = new Mock<IListFeatureStatesQueryHandler>();
        list.Setup(h => h.HandleAsync(It.IsAny<ListFeatureStatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<FeatureState>()));

        var controller = new FeatureStatesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, null, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task List_WithItems_Returns200WithResponses()
    {
        var create = new Mock<ICreateFeatureStateCommandHandler>();
        var update = new Mock<IUpdateFeatureStateCommandHandler>();
        var delete = new Mock<IDeleteFeatureStateCommandHandler>();
        var getById = new Mock<IGetFeatureStateByIdQueryHandler>();
        var list = new Mock<IListFeatureStatesQueryHandler>();
        var item = new FeatureState { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = true, Reason = "" };
        list.Setup(h => h.HandleAsync(It.IsAny<ListFeatureStatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<FeatureState> { item }));

        var controller = new FeatureStatesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<List<FeatureStateResponse>>(ok.Value);
        Assert.Single(payload);
        Assert.Equal(item.Id, payload[0].Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var create = new Mock<ICreateFeatureStateCommandHandler>();
        var update = new Mock<IUpdateFeatureStateCommandHandler>();
        var delete = new Mock<IDeleteFeatureStateCommandHandler>();
        var getById = new Mock<IGetFeatureStateByIdQueryHandler>();
        getById.Setup(h => h.HandleAsync(It.IsAny<GetFeatureStateByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<FeatureState>("NotFound"));
        var list = new Mock<IListFeatureStatesQueryHandler>();

        var controller = new FeatureStatesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Failed_ReturnsProblem500()
    {
        var create = new Mock<ICreateFeatureStateCommandHandler>();
        create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateFeatureStateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<FeatureState>("error"));
        var update = new Mock<IUpdateFeatureStateCommandHandler>();
        var delete = new Mock<IDeleteFeatureStateCommandHandler>();
        var getById = new Mock<IGetFeatureStateByIdQueryHandler>();
        var list = new Mock<IListFeatureStatesQueryHandler>();

        var controller = new FeatureStatesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var req = new CreateFeatureStateRequest { FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Enabled = true, Reason = null };
        var action = await controller.Create(req, CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
    }
}