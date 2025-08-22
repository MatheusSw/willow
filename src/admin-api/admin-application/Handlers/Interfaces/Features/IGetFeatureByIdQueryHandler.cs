using admin_application.Queries;
using admin_domain.Entities;
using FluentResults;

namespace admin_application.Handlers.Interfaces.Features;

public interface IGetFeatureByIdQueryHandler
{
    Task<Result<Feature>> HandleAsync(GetFeatureByIdQuery query, CancellationToken cancellationToken);
}


