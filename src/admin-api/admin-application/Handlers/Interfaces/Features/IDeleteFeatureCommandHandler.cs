using admin_application.Commands;

using FluentResults;

namespace admin_application.Handlers.Interfaces.Features;

public interface IDeleteFeatureCommandHandler
{
	Task<Result> HandleAsync(DeleteFeatureCommand command, CancellationToken cancellationToken);
}