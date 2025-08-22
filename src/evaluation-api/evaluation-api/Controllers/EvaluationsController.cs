using evaluation_application.Queries;
using evaluation_application.Interfaces;
using evaluation_api.DTOs.Request;
using evaluation_api.DTOs.Response;
using Serilog;
using Microsoft.AspNetCore.Mvc;

namespace evaluation_api.Controllers;

[ApiController]
[Route("v1/evaluate")]
/// <summary>
/// Handles evaluation requests for feature flags. Validates API key and delegates evaluation to the application layer.
/// </summary>
public sealed class EvaluationsController(
    IEvaluateFeatureQueryHandler handler,
    IValidateApiKeyQueryHandler apiKeyHandler)
    : ControllerBase
{
    private const string ApiKeyHeader = "x-api-key";

    [HttpPost]
    /// <summary>
    /// Evaluates whether a feature is enabled for the provided attributes.
    /// </summary>
    /// <param name="request">The request payload containing project, environment, feature and attributes.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>HTTP 200 with evaluation result, or 401 when API key is missing or invalid.</returns>
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


