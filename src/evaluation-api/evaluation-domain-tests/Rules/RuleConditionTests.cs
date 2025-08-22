using AutoFixture;
using evaluation_domain.Rules;
using Xunit;

namespace evaluation_domain_tests.Rules;

public static class FixtureFactory
{
	public static Fixture Create()
	{
		return new Fixture();
	}
}

public class RuleConditionTests
{
	[Fact]
	public void Properties_SetAndGet_Work()
	{
		// Arrange
		var fixture = FixtureFactory.Create();
		var attribute = fixture.Create<string>();
		var value = fixture.Create<string>();

		var condition = new RuleCondition
		{
			Attribute = attribute,
			Op = "equals",
			Value = value
		};

		// Act

		// Assert
		Assert.Equal(attribute, condition.Attribute);
		Assert.Equal("equals", condition.Op);
		Assert.Equal(value, condition.Value);
	}
}


