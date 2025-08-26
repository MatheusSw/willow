using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace admin_infrastructure.Db;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FeatureToggleDbContext>
{
    public FeatureToggleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FeatureToggleDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=feature_toggle;Username=postgres;Password=postgres;");
        return new FeatureToggleDbContext(optionsBuilder.Options);
    }
}