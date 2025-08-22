using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using evaluation_application.Interfaces;
using evaluation_application.Queries;
using evaluation_application.Services;
using Serilog;

namespace evaluation_application.Handlers;

/// <summary>
/// Handles feature evaluation queries by loading configuration and applying rule evaluation.
/// </summary>
public sealed class EvaluateFeatureQueryHandler : IEvaluateFeatureQueryHandler
{
    private readonly IFeatureConfigRepository _featureConfigRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluateFeatureQueryHandler"/> class.
    /// </summary>
    /// <param name="featureConfigRepository">Repository for fetching feature configurations.</param>
    public EvaluateFeatureQueryHandler(IFeatureConfigRepository featureConfigRepository)
    {
        _featureConfigRepository = featureConfigRepository;
    }

    /// <inheritdoc />
    public async Task<EvaluateFeatureResult> HandleAsync(EvaluateFeatureQuery query, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<EvaluateFeatureQueryHandler>()
            .ForContext("project", query.Project)
            .ForContext("environment", query.Environment)
            .ForContext("feature", query.Feature);
        log.Information("Handle started");

        var (found, config) = await _featureConfigRepository.TryGetConfigAsync(query.Project, query.Environment, query.Feature, cancellationToken);
        if (!found || config is null)
        {
            log.Information("Feature config not found");
            return new EvaluateFeatureResult
            {
                Enabled = false,
                Reason = "not_found",
                Variant = null
            };
        }

        var (enabled, reason) = RuleEvaluator.Evaluate(config, query.Attributes);

        var result = new EvaluateFeatureResult
        {
            Enabled = enabled,
            Reason = reason,
            Variant = null
        };
        log.Information("Handle completed: Enabled={Enabled} Reason={Reason}", result.Enabled, result.Reason);
        return result;
    }
}


