using admin_application.Commands;

using admin_domain.Entities;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Features;

public interface IUpdateFeatureCommandHandler
{
    Task<Result<Feature>> HandleAsync(UpdateFeatureCommand command, CancellationToken cancellationToken);
}