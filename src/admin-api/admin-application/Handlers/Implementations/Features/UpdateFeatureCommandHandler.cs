using admin_application.Commands;
using admin_application.Handlers.Interfaces.Features;
using admin_application.Interfaces;
using admin_domain.Entities;
using FluentResults;
using Serilog;

namespace admin_application.Handlers.Implementations.Features;

public sealed class UpdateFeatureCommandHandler : IUpdateFeatureCommandHandler
{
    private readonly IFeatureRepository _repository;
    public UpdateFeatureCommandHandler(IFeatureRepository repository) { _repository = repository; }

    public async Task<Result<Feature>> HandleAsync(UpdateFeatureCommand command, CancellationToken cancellationToken)
    {
        var log = Log.ForContext<UpdateFeatureCommandHandler>()
            .ForContext("Id", command.Id)
            .ForContext("ProjectId", command.ProjectId)
            .ForContext("Name", command.Name);
        log.Information("UpdateFeature started");
        var model = new Feature { Id = command.Id, ProjectId = command.ProjectId, Name = command.Name, Description = command.Description };
        var result = await _repository.UpdateAsync(model, cancellationToken);
        log.Information("UpdateFeature completed: {Success}", result.IsSuccess);
        return result;
    }
}


