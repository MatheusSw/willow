using AutoFixture;
using AutoFixture.AutoMoq;

namespace admin_api_tests.Controllers;

public static class FixtureFactory
{
	public static IFixture Create()
	{
		return new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
	}
}


