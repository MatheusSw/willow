using admin_application.Commands;
using admin_application.Handlers.Interfaces.Rules;
using admin_application.Interfaces;

using admin_domain.Entities;
using admin_domain.Rules;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Rules;

public sealed class UpdateRuleCommandHandler(IRuleRepository repository) : IUpdateRuleCommandHandler
{
	public async Task<Result<Rule>> HandleAsync(UpdateRuleCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<UpdateRuleCommandHandler>()
			.ForContext("Id", command.Id)
			.ForContext("FeatureId", command.FeatureId)
			.ForContext("EnvironmentId", command.EnvironmentId)
			.ForContext("Priority", command.Priority)
			.ForContext("MatchType", command.MatchType);

		log.Information("UpdateRule started");

		var model = new Rule
		{
			Id = command.Id,
			FeatureId = command.FeatureId,
			EnvironmentId = command.EnvironmentId,
			Priority = command.Priority,
			MatchType = string.Equals(command.MatchType, "any", StringComparison.OrdinalIgnoreCase) ? admin_domain.Rules.MatchType.Any : admin_domain.Rules.MatchType.All,
			Conditions = command.Conditions
		};

		var result = await repository.UpdateAsync(model, cancellationToken);

		log.Information("UpdateRule completed: {Success}", result.IsSuccess);

		return result;
	}
}