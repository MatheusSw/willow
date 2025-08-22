using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace evaluation_infrastructure.Db;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FeatureToggleDbContext>
{
    public FeatureToggleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FeatureToggleDbContext>();

        //TODO: Fix this to use the actual connection string from appsettings or environment
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=feature_toggle;Username=postgres;Password=postgres;");
        return new FeatureToggleDbContext(optionsBuilder.Options);
    }
}


