using Serilog;
using Serilog.Formatting.Compact;
using Microsoft.EntityFrameworkCore;
using admin_infrastructure.Db;
using admin_infrastructure.Models.Redis;
using admin_application;
using admin_infrastructure;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        .WriteTo.Async(config =>
        {
#if !DEBUG
            config.Console(new CompactJsonFormatter());
#else
            config.Console();
#endif
        })
        .CreateLogger();

    Log.Information("Starting up admin-api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddDbContext<FeatureToggleDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    });

    builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
        options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "admin-api:";
    });

    builder.Services.AddAdminApplication();
    builder.Services.AddAdminInfrastructure();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "admin-api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
