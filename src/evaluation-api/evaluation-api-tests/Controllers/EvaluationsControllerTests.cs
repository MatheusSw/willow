using AutoFixture;
using AutoFixture.AutoMoq;
using evaluation_api.Controllers;
using evaluation_api.DTOs.Request;
using evaluation_api.DTOs.Response;
using evaluation_application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace evaluation_api_tests.Controllers;

public static class FixtureFactory
{
	public static IFixture Create()
	{
		return new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
	}
}

public class EvaluationsControllerTests
{
	[Fact]
	public async Task Evaluate_ValidRequestUnauthorizedKey_Returns401()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var evalHandler = fixture.Create<Mock<IEvaluateFeatureQueryHandler>>();
		var apiKeyHandler = fixture.Create<Mock<IValidateApiKeyQueryHandler>>();
		apiKeyHandler.Setup(h => h.HandleAsync(It.IsAny<ValidateApiKeyQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var controller = new EvaluationsController(evalHandler.Object, apiKeyHandler.Object)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};
		controller.ControllerContext.HttpContext.Request.Headers["x-api-key"] = fixture.Create<string>();

		var req = new EvaluateRequest
		{
			Project = fixture.Create<string>(),
			Environment = fixture.Create<string>(),
			Feature = fixture.Create<string>(),
			Attributes = new System.Collections.Generic.Dictionary<string, string>()
		};

		// Act
		var result = await controller.Evaluate(req, CancellationToken.None);

		// Assert
		Assert.IsType<UnauthorizedResult>(result.Result);
	}

	[Fact]
	public async Task Evaluate_MissingApiKeyHeader_Returns401()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var evalHandler = fixture.Create<Mock<IEvaluateFeatureQueryHandler>>();
		var apiKeyHandler = fixture.Create<Mock<IValidateApiKeyQueryHandler>>();

		var controller = new EvaluationsController(evalHandler.Object, apiKeyHandler.Object)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};

		var req = new EvaluateRequest
		{
			Project = fixture.Create<string>(),
			Environment = fixture.Create<string>(),
			Feature = fixture.Create<string>(),
			Attributes = new System.Collections.Generic.Dictionary<string, string>()
		};

		// Act
		var result = await controller.Evaluate(req, CancellationToken.None);

		// Assert
		Assert.IsType<UnauthorizedResult>(result.Result);
	}

	[Fact]
	public async Task Evaluate_ValidRequestAndKey_Returns200WithResponse()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var evalHandler = fixture.Create<Mock<IEvaluateFeatureQueryHandler>>();
		var apiKeyHandler = fixture.Create<Mock<IValidateApiKeyQueryHandler>>();
		apiKeyHandler.Setup(h => h.HandleAsync(It.IsAny<ValidateApiKeyQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		var expected = new EvaluateFeatureResult
		{
			Enabled = true,
			Reason = "rule: orgId=acme",
			Variant = null
		};
		evalHandler.Setup(h => h.HandleAsync(It.IsAny<EvaluateFeatureQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expected);

		var controller = new EvaluationsController(evalHandler.Object, apiKeyHandler.Object)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};
		controller.ControllerContext.HttpContext.Request.Headers["x-api-key"] = fixture.Create<string>();

		var req = new EvaluateRequest
		{
			Project = fixture.Create<string>(),
			Environment = fixture.Create<string>(),
			Feature = fixture.Create<string>(),
			Attributes = new System.Collections.Generic.Dictionary<string, string>()
		};

		// Act
		var actionResult = await controller.Evaluate(req, CancellationToken.None);

		// Assert
		var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
		var response = Assert.IsType<EvaluateResponse>(ok.Value);
		Assert.Equal(expected.Enabled, response.Enabled);
		Assert.Equal(expected.Reason, response.Reason);
		Assert.Equal(expected.Variant, response.Variant);
	}
}


