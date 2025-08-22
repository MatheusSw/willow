using admin_api.Controllers;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;
using admin_application.Handlers.Interfaces.Projects;
using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace admin_api_tests.Controllers;

public class ProjectsControllerTests
{
	[Fact]
	public async Task List_NoItems_Returns204()
	{
		var create = new Mock<ICreateProjectCommandHandler>();
		var update = new Mock<IUpdateProjectCommandHandler>();
		var delete = new Mock<IDeleteProjectCommandHandler>();
		var getById = new Mock<IGetProjectByIdQueryHandler>();
		var list = new Mock<IListProjectsQueryHandler>();
		list.Setup(h => h.HandleAsync(It.IsAny<ListProjectsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(new List<Project>()));

		var controller = new ProjectsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

		var result = await controller.List(null, CancellationToken.None);

		Assert.IsType<NoContentResult>(result.Result);
	}

	[Fact]
	public async Task List_WithItems_Returns200WithResponses()
	{
		var create = new Mock<ICreateProjectCommandHandler>();
		var update = new Mock<IUpdateProjectCommandHandler>();
		var delete = new Mock<IDeleteProjectCommandHandler>();
		var getById = new Mock<IGetProjectByIdQueryHandler>();
		var list = new Mock<IListProjectsQueryHandler>();
		var item = new Project { Id = Guid.NewGuid(), OrgId = Guid.NewGuid(), Name = "proj" };
		list.Setup(h => h.HandleAsync(It.IsAny<ListProjectsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Ok(new List<Project> { item }));

		var controller = new ProjectsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

		var result = await controller.List(null, CancellationToken.None);

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		var payload = Assert.IsType<List<ProjectResponse>>(ok.Value);
		Assert.Single(payload);
		Assert.Equal(item.Id, payload[0].Id);
	}

	[Fact]
	public async Task GetById_NotFound_Returns404()
	{
		var create = new Mock<ICreateProjectCommandHandler>();
		var update = new Mock<IUpdateProjectCommandHandler>();
		var delete = new Mock<IDeleteProjectCommandHandler>();
		var getById = new Mock<IGetProjectByIdQueryHandler>();
		getById.Setup(h => h.HandleAsync(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Fail<Project>("NotFound"));
		var list = new Mock<IListProjectsQueryHandler>();

		var controller = new ProjectsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

		var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

		Assert.IsType<NotFoundResult>(result.Result);
	}

	[Fact]
	public async Task Create_Failed_ReturnsProblem500()
	{
		var create = new Mock<ICreateProjectCommandHandler>();
		create.Setup(h => h.HandleAsync(It.IsAny<admin_application.Commands.CreateProjectCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Fail<Project>("error"));
		var update = new Mock<IUpdateProjectCommandHandler>();
		var delete = new Mock<IDeleteProjectCommandHandler>();
		var getById = new Mock<IGetProjectByIdQueryHandler>();
		var list = new Mock<IListProjectsQueryHandler>();

		var controller = new ProjectsController(create.Object, update.Object, delete.Object, getById.Object, list.Object);

		var action = await controller.Create(new CreateProjectRequest { OrgId = Guid.NewGuid(), Name = "proj" }, CancellationToken.None);

		var problem = Assert.IsType<ObjectResult>(action.Result);
		Assert.Equal(500, problem.StatusCode);
	}
}


