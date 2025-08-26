using admin_application.Commands;
using admin_application.Handlers.Interfaces;
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
    IPublishFeatureStateUpdatedCommandHandler publishFeatureStateUpdatedHandler) : IUpdateFeatureStateCommandHandler
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

        var model = new FeatureState
        {
            Id = command.Id, FeatureId = command.FeatureId, EnvironmentId = command.EnvironmentId,
            Enabled = command.Enabled, Reason = command.Reason
        };
        
        var updateResult = await featureStateRepository.UpdateAsync(model, cancellationToken);
        if (updateResult.IsFailed)
        {
            log
                .ForContext("Update Result", updateResult, true)
                .Error("Failed to update feature state in database");

            return updateResult;
        }

        var featureResult = await featureRepository.GetByIdAsync(command.FeatureId, cancellationToken);

        if (ResultHelpers.TryGetFailure(featureResult,
                $"Feature not found: {command.FeatureId}", out Result<FeatureState>? featureFailure))
        {
            return featureFailure;
        }

        var feature = featureResult.Value;

        var publishCommand = new PublishFeatureStateUpdatedCommand
        {
            ProjectId = feature.ProjectId,
            FeatureName = feature.Name,
            Enabled = command.Enabled
        };

        await publishFeatureStateUpdatedHandler.HandleAsync(publishCommand, cancellationToken);

        log.Information("UpdateFeatureState completed: {Success}", updateResult.IsSuccess);

        return updateResult;
    }
}