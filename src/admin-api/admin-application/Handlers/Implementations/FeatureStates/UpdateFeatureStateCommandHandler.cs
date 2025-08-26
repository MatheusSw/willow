using admin_application.Commands;
using admin_application.Events;
using admin_application.Handlers.Interfaces.FeatureStates;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Serilog;
using admin_application.Utilities;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class UpdateFeatureStateCommandHandler(
    IFeatureStateRepository featureStateRepository,
    IFeatureRepository featureRepository,
    IEnvironmentRepository environmentRepository,
    IEventPublisher eventPublisher,
    ICacheInvalidator cacheInvalidator) : IUpdateFeatureStateCommandHandler
{
    public async Task<Result<FeatureState>> HandleAsync(UpdateFeatureStateCommand command,
        CancellationToken cancellationToken)
    {
        var log = Log.ForContext<UpdateFeatureStateCommandHandler>()
            .ForContext("Id", command.Id)
            .ForContext("FeatureId", command.FeatureId)
            .ForContext("EnvironmentId", command.EnvironmentId)
            .ForContext("Enabled", command.Enabled);

        log.Information("UpdateFeatureState started");

        // Create and update feature state
        var model = new FeatureState
        {
            Id = command.Id, FeatureId = command.FeatureId, EnvironmentId = command.EnvironmentId,
            Enabled = command.Enabled, Reason = command.Reason
        };
        var updateResult = await featureStateRepository.UpdateAsync(model, cancellationToken);

        if (updateResult.IsFailed)
        {
            log.Error("Failed to update feature state in database");

            return updateResult;
        }

        var featureTask = featureRepository.GetByIdAsync(command.FeatureId, cancellationToken);
        var environmentTask = environmentRepository.GetByIdAsync(command.EnvironmentId, cancellationToken);

        await Task.WhenAll(featureTask, environmentTask);

        var featureResult = featureTask.Result;
        var environmentResult = environmentTask.Result;

        if (ResultHelpers.TryGetFailure(featureResult,
                $"Feature not found: {command.FeatureId}", out Result<FeatureState>? featureFailure))
        {
            return featureFailure;
        }

        var feature = featureResult.Value;

        if (ResultHelpers.TryGetFailure(environmentResult,
                $"Environment not found: {command.EnvironmentId}", out Result<FeatureState>? environmentFailure))
        {
            return environmentFailure;
        }

        var environment = environmentResult.Value;

        try
        {
            var featureStateEvent = new FeatureStateUpdatedEvent
            {
                ProjectId = $"proj-{feature.ProjectId}",
                Environment = environment.Key,
                Feature = feature.Name,
                Enabled = command.Enabled,
                Timestamp = DateTime.UtcNow
            };

            await eventPublisher.PublishAsync("ft:updates", featureStateEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            log.Error(ex, "Failed to publish feature state updated event");
        }

        try
        {
            var cacheKey = $"ft:cfg:{feature.ProjectId}:{environment.Key}:{feature.Name}";

            await cacheInvalidator.InvalidateAsync(cacheKey, cancellationToken);
        }
        catch (Exception ex)
        {
            log.Error(ex, "Failed to invalidate cache for feature config");
        }

        log.Information("UpdateFeatureState completed: {Success}", updateResult.IsSuccess);

        return updateResult;
    }
}