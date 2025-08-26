using admin_application.Commands;
using admin_application.Events;
using admin_application.Handlers.Interfaces;
using admin_application.Interfaces;
using admin_domain;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.FeatureStates;

public sealed class PublishFeatureStateUpdatedCommandHandler(
    IEventPublisher eventPublisher,
    ICacheInvalidator cacheInvalidator) : IPublishFeatureStateUpdatedCommandHandler
{
    public async Task<Result> HandleAsync(PublishFeatureStateUpdatedCommand command,
        CancellationToken cancellationToken = default)
    {
        var log = Log.ForContext<PublishFeatureStateUpdatedCommandHandler>()
            .ForContext("ProjectId", command.ProjectId)
            .ForContext("Environment", command)
            .ForContext("Feature", command.FeatureName)
            .ForContext("Enabled", command.Enabled);

        log.Information("Publishing feature state updated event and invalidating cache started");

        try
        {
            var featureStateEvent = new FeatureStateUpdatedEvent
            {
                ProjectId = command.ProjectId,
                Feature = command.FeatureName,
                Enabled = command.Enabled,
                Timestamp = DateTimeOffset.Now
            };

            await eventPublisher.PublishAsync("ft:updates", featureStateEvent, cancellationToken);

            log.Information("Feature state updated event published successfully");
        }
        catch (Exception ex)
        {
            log.Error(ex, "Failed to publish feature state updated event");
        }

        try
        {
            var cacheKey = CacheKeys.FeatureConfig(command.ProjectId, command.FeatureName);

            await cacheInvalidator.InvalidateAsync(cacheKey, cancellationToken);

            log.Information("Cache invalidated successfully for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            log.Error(ex, "Failed to invalidate cache for feature config");
        }

        log.Information("Publishing feature state updated event and invalidating cache completed");

        return Result.Ok();
    }
}