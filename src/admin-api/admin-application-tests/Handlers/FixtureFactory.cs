using AutoFixture;
using AutoFixture.AutoMoq;

namespace admin_application_tests.Handlers;

public static class FixtureFactory
{
    public static IFixture Create() => new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
}


