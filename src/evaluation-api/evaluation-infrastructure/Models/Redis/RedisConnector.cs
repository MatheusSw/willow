using Serilog;
using StackExchange.Redis;

namespace evaluation_infrastructure.Models.Redis;

internal static class RedisConnector
{
    public static async Task<ConnectionMultiplexer> ConnectWithRetryAsync(RedisSettings settings)
    {
        var attempt = 0;
        var delayMs = Math.Max(50, settings.InitialRetryDelayMs);

        while (true)
        {
            attempt++;
            try
            {
                var options = ConfigurationOptions.Parse(settings.Configuration ?? "localhost:6379");
                options.AbortOnConnectFail = false;
                options.ConnectRetry = Math.Max(0, settings.ConnectRetry);
                options.ConnectTimeout = Math.Max(1000, settings.ConnectTimeoutMs);

                Log.Information("Connecting to Redis: {RedisConfiguration}, attempt {Attempt}", settings.Configuration, attempt);

                var mux = await ConnectionMultiplexer.ConnectAsync(options);
                if (!mux.IsConnected)
                {
                    throw new RedisConnectionException(ConnectionFailureType.UnableToResolvePhysicalConnection, "Failed to establish connection (not connected)");
                }

                Log.Information("Connected to Redis");
                return mux;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Redis connection attempt {Attempt} failed", attempt);

                if (attempt >= Math.Max(1, settings.MaxConnectAttempts))
                {
                    Log.Error("Exceeded maximum Redis connection attempts: {MaxAttempts}", settings.MaxConnectAttempts);
                    throw;
                }

                await Task.Delay(delayMs);
                delayMs = Math.Min(delayMs * 2, 30_000);
            }
        }
    }
}