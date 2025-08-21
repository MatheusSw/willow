using evaluation_application.Queries;
using evaluation_application.Interfaces;
using evaluation_api.DTOs.Request;
using evaluation_api.DTOs.Response;
using Serilog;
using Microsoft.AspNetCore.Mvc;

namespace evaluation_api.Controllers;

[ApiController]
[Route("v1/evaluate")]
public sealed class EvaluationsController(
    IEvaluateFeatureQueryHandler handler,
    IValidateApiKeyQueryHandler apiKeyHandler)
    : ControllerBase
{
    private const string ApiKeyHeader = "x-api-key";

    [HttpPost]
    public async Task<ActionResult<EvaluateResponse>> Evaluate([FromBody] EvaluateRequest request, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EvaluationsController>()
            .ForContext("project", request.Project)
            .ForContext("environment", request.Environment)
            .ForContext("feature", request.Feature);

        log.Information("Evaluate started");

        if (!Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
        {
            log.Warning("Missing API key header");

            return Unauthorized();
        }

        var valid = await apiKeyHandler.HandleAsync(new ValidateApiKeyQuery { ApiKey = apiKey! }, cancellationToken);
        if (!valid)
        {
            log.Warning("API key invalid");

            return Unauthorized();
        }

        var result = await handler.HandleAsync(new EvaluateFeatureQuery
        {
            Project = request.Project,
            Environment = request.Environment,
            Feature = request.Feature,
            Attributes = request.Attributes
        }, cancellationToken);

        log.Information("Evaluate completed: Enabled={Enabled} Reason={Reason}", result.Enabled, result.Reason);

        return Ok(new EvaluateResponse
        {
            Enabled = result.Enabled,
            Reason = result.Reason,
            Variant = result.Variant
        });
    }
}


