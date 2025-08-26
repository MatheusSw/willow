using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Handlers.Interfaces.Features;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace admin_api_tests.Controllers;

public class FeaturesControllerTests
{
    [Fact]
    public async Task List_NoItems_Returns204()
    {
        var create = new Mock<ICreateFeatureCommandHandler>();
        var update = new Mock<IUpdateFeatureCommandHandler>();
        var delete = new Mock<IDeleteFeatureCommandHandler>();
        var getById = new Mock<IGetFeatureByIdQueryHandler>();
        var list = new Mock<IListFeaturesQueryHandler>();
        list.Setup(h => h.HandleAsync(It.IsAny<ListFeaturesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Feature>()));

        var controller = new FeaturesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task List_WithItems_Returns200WithResponses()
    {
        var create = new Mock<ICreateFeatureCommandHandler>();
        var update = new Mock<IUpdateFeatureCommandHandler>();
        var delete = new Mock<IDeleteFeatureCommandHandler>();
        var getById = new Mock<IGetFeatureByIdQueryHandler>();
        var list = new Mock<IListFeaturesQueryHandler>();
        var item = new Feature { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "feat", Description = "desc" };
        list.Setup(h => h.HandleAsync(It.IsAny<ListFeaturesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Feature> { item }));

        var controller = new FeaturesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<List<FeatureResponse>>(ok.Value);
        Assert.Single(payload);
        Assert.Equal(item.Id, payload[0].Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var create = new Mock<ICreateFeatureCommandHandler>();
        var update = new Mock<IUpdateFeatureCommandHandler>();
        var delete = new Mock<IDeleteFeatureCommandHandler>();
        var getById = new Mock<IGetFeatureByIdQueryHandler>();
        getById.Setup(h => h.HandleAsync(It.IsAny<GetFeatureByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Feature>("NotFound"));
        var list = new Mock<IListFeaturesQueryHandler>();

        var controller = new FeaturesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Failed_ReturnsProblem500()
    {
        var create = new Mock<ICreateFeatureCommandHandler>();
        create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateFeatureCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Feature>("error"));
        var update = new Mock<IUpdateFeatureCommandHandler>();
        var delete = new Mock<IDeleteFeatureCommandHandler>();
        var getById = new Mock<IGetFeatureByIdQueryHandler>();
        var list = new Mock<IListFeaturesQueryHandler>();

        var controller = new FeaturesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var req = new CreateFeatureRequest { ProjectId = Guid.NewGuid(), Name = "feat", Description = "desc" };
        var action = await controller.Create(req, CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
    }
}