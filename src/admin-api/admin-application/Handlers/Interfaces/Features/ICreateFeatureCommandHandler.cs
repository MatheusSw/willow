using admin_application.Commands;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Features;

public interface ICreateFeatureCommandHandler
{
    Task<Result<Feature>> HandleAsync(CreateFeatureCommand command, CancellationToken cancellationToken);
}


