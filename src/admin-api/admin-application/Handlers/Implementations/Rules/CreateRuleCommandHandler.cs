using admin_application.Commands;
using admin_application.Handlers.Interfaces.Rules;
using admin_application.Interfaces;

using admin_domain.Entities;
using admin_domain.Rules;

using FluentResults;

using Serilog;

namespace admin_application.Handlers.Implementations.Rules;

public sealed class CreateRuleCommandHandler(IRuleRepository repository) : ICreateRuleCommandHandler
{
	public async Task<Result<Rule>> HandleAsync(CreateRuleCommand command, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<CreateRuleCommandHandler>()
			.ForContext("FeatureId", command.FeatureId)
			.ForContext("EnvironmentId", command.EnvironmentId)
			.ForContext("Priority", command.Priority)
			.ForContext("MatchType", command.MatchType);

		log.Information("CreateRule started");

		var model = new Rule
		{
			Id = Guid.NewGuid(),
			FeatureId = command.FeatureId,
			EnvironmentId = command.EnvironmentId,
			Priority = command.Priority,
			MatchType = string.Equals(command.MatchType, "any", StringComparison.OrdinalIgnoreCase) ? admin_domain.Rules.MatchType.Any : admin_domain.Rules.MatchType.All,
			Conditions = command.Conditions
		};

		var result = await repository.CreateAsync(model, cancellationToken);

		log.Information("CreateRule completed: {Success}", result.IsSuccess);

		return result;
	}
}