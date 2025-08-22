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

	[Fact]
	public void Evaluate_AnyMatch_WithSingleMatchingCondition_ReturnsEnabledTrue()
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
					MatchType = "any",
					Conditions =
					[
						new evaluation_domain.Rules.RuleCondition { Attribute = "orgId", Op = "equals", Value = "acme" },
						new evaluation_domain.Rules.RuleCondition { Attribute = "userId", Op = "equals", Value = "bob" }
					]
				}
			]
		};
		var attrs = new Dictionary<string, string> { { "userId", "bob" } };

		// Act
		var (enabled, reason) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.True(enabled);
		Assert.Contains("rule:", reason);
	}

	[Fact]
	public void Evaluate_NoRuleMatches_ReturnsDefaultFromConfig()
	{
		// Arrange
		var config = new FeatureConfig
		{
			Enabled = true,
			Reason = null,
			Rules = []
		};
		var attrs = new Dictionary<string, string>();

		// Act
		var (enabled, reason) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.True(enabled);
		Assert.Equal("default:true", reason);
	}

	[Fact]
	public void Evaluate_NoRuleMatches_CustomReasonReturned()
	{
		// Arrange
		var config = new FeatureConfig
		{
			Enabled = false,
			Reason = "maintenance",
			Rules = []
		};
		var attrs = new Dictionary<string, string>();

		// Act
		var (enabled, reason) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.False(enabled);
		Assert.Equal("maintenance", reason);
	}

	[Fact]
	public void Evaluate_Equals_IsCaseInsensitive()
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
						new evaluation_domain.Rules.RuleCondition { Attribute = "orgId", Op = "equals", Value = "ACME" }
					]
				}
			]
		};
		var attrs = new Dictionary<string, string> { { "orgId", "acme" } };

		// Act
		var (enabled, _) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.True(enabled);
	}

	[Fact]
	public void Evaluate_UnknownOp_DoesNotMatch_ReturnsDefault()
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
						new evaluation_domain.Rules.RuleCondition { Attribute = "orgId", Op = "startsWith", Value = "ac" }
					]
				}
			]
		};
		var attrs = new Dictionary<string, string> { { "orgId", "acme" } };

		// Act
		var (enabled, reason) = RuleEvaluator.Evaluate(config, attrs);

		// Assert
		Assert.False(enabled);
		Assert.Equal("default:false", reason);
	}
}