using admin_application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;

namespace admin_infrastructure.Services.Redis;

public sealed class RedisEventPublisher : IEventPublisher
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisEventPublisher(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task PublishAsync<T>(string channel, T eventData, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            throw new ArgumentException("Channel cannot be null or whitespace.", nameof(channel));
        }

        ArgumentNullException.ThrowIfNull(eventData);

        var log = Log.ForContext<RedisEventPublisher>()
            .ForContext("Channel", channel)
            .ForContext("EventType", typeof(T).Name);

        try
        {
            var database = _connectionMultiplexer.GetSubscriber();

            var jsonData = JsonSerializer.Serialize(eventData);

            log.Information("Publishing event to Redis channel");

            var publishedCount = await database.PublishAsync(new RedisChannel(channel, RedisChannel.PatternMode.Pattern), jsonData);

            log.Information("Event published successfully to {SubscriberCount} subscribers", publishedCount);
        }
        catch (Exception ex)
        {
            log.Error(ex, "Failed to publish event to Redis channel");

            throw;
        }
    }
}
