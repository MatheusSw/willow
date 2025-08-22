using Serilog;
using Serilog.Formatting.Compact;
using Microsoft.EntityFrameworkCore;
using evaluation_infrastructure.Db;
using evaluation_application;
using evaluation_application.Interfaces;
using evaluation_infrastructure.Repositories;
using evaluation_infrastructure.Models.Redis;

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

    Log.Information("Starting up evaluation-api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddDbContext<FeatureToggleDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    });

    // Distributed cache (Redis)
    builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
        options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "evaluation-api:";
    });

    // Application and Infrastructure services
    builder.Services.AddEvaluationApplication();
    builder.Services.AddScoped<IFeatureConfigRepository, FeatureConfigRepository>();
    builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

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
    Log.Fatal(ex, "evaluation-api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}