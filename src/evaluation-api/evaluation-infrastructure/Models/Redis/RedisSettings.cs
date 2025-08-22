namespace evaluation_infrastructure.Models.Redis;

/// <summary>
/// Redis connection settings bound from configuration.
/// </summary>
public sealed class RedisSettings
{
    /// <summary>
    /// Redis configuration string (e.g., host:port,password=... ).
    /// </summary>
    public string? Configuration { get; set; }
    /// <summary>
    /// Prefix used for keys created by this service.
    /// </summary>
    public string? InstanceName { get; set; }
    /// <summary>
    /// Number of retry attempts for a single connect operation.
    /// </summary>
    public int ConnectRetry { get; set; } = 3;
    /// <summary>
    /// Connect timeout in milliseconds.
    /// </summary>
    public int ConnectTimeoutMs { get; set; } = 5000;
    /// <summary>
    /// Maximum number of connection attempts with backoff before giving up.
    /// </summary>
    public int MaxConnectAttempts { get; set; } = 5;
    /// <summary>
    /// Initial delay used for exponential backoff between connection attempts.
    /// </summary>
    public int InitialRetryDelayMs { get; set; } = 200;
}