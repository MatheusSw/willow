using admin_application.Commands;
using FluentResults;

namespace admin_application.Handlers.Interfaces;

public interface IPublishFeatureStateUpdatedCommandHandler
{
    Task<Result> HandleAsync(PublishFeatureStateUpdatedCommand command, CancellationToken cancellationToken = default);
}
