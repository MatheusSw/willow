using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;

using admin_application.Handlers.Interfaces.Rules;
using admin_application.Queries;

using admin_domain.Entities;

using FluentResults;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace admin_api_tests.Controllers;

public class RulesControllerTests
{
    [Fact]
    public async Task List_NoItems_Returns204()
    {
        var create = new Mock<ICreateRuleCommandHandler>();
        var update = new Mock<IUpdateRuleCommandHandler>();
        var delete = new Mock<IDeleteRuleCommandHandler>();
        var getById = new Mock<IGetRuleByIdQueryHandler>();
        var list = new Mock<IListRulesQueryHandler>();
        list.Setup(h => h.HandleAsync(It.IsAny<ListRulesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Rule>()));

        var controller = new RulesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, null, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task List_WithItems_Returns200WithResponses()
    {
        var create = new Mock<ICreateRuleCommandHandler>();
        var update = new Mock<IUpdateRuleCommandHandler>();
        var delete = new Mock<IDeleteRuleCommandHandler>();
        var getById = new Mock<IGetRuleByIdQueryHandler>();
        var list = new Mock<IListRulesQueryHandler>();
        var item = new Rule { Id = Guid.NewGuid(), FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1, MatchType = admin_domain.Rules.MatchType.All, Conditions = [] };
        list.Setup(h => h.HandleAsync(It.IsAny<ListRulesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<Rule> { item }));

        var controller = new RulesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.List(null, null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<List<RuleResponse>>(ok.Value);
        Assert.Single(payload);
        Assert.Equal(item.Id, payload[0].Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var create = new Mock<ICreateRuleCommandHandler>();
        var update = new Mock<IUpdateRuleCommandHandler>();
        var delete = new Mock<IDeleteRuleCommandHandler>();
        var getById = new Mock<IGetRuleByIdQueryHandler>();
        getById.Setup(h => h.HandleAsync(It.IsAny<GetRuleByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Rule>("NotFound"));
        var list = new Mock<IListRulesQueryHandler>();

        var controller = new RulesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Failed_ReturnsProblem500()
    {
        var create = new Mock<ICreateRuleCommandHandler>();
        create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateRuleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Rule>("error"));
        var update = new Mock<IUpdateRuleCommandHandler>();
        var delete = new Mock<IDeleteRuleCommandHandler>();
        var getById = new Mock<IGetRuleByIdQueryHandler>();
        var list = new Mock<IListRulesQueryHandler>();

        var controller = new RulesController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

        var req = new CreateRuleRequest { FeatureId = Guid.NewGuid(), EnvironmentId = Guid.NewGuid(), Priority = 1, MatchType = "all", Conditions = [] };
        var action = await controller.Create(req, CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
    }
}