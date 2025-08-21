using evaluation_application.Interfaces;
using evaluation_application.Services;

namespace evaluation_application_tests;

public class RuleEvaluatorTests
{
	[Fact]
	public void Evaluate_AllMatch_ReturnsEnabledTrueWithRuleReason()
	{
		// Arrange
		var config = new FeatureConfig
		{
			Enabled = false,
			Rules =
			[
				new FeatureRule
				{
					Priority = 1,
					MatchType = "all",
					Conditions =
					[
						new evaluation_domain.Rules.RuleCondition { Attribute = "orgId", Op = "equals", Value = "acme" }
					]
				}
			]
		};
		var attrs = new Dictionary<string, string> { { "orgId", "acme" } };

		// Act
		var (enabled, reason) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.True(enabled);
		Assert.Contains("rule:", reason);
	}
}