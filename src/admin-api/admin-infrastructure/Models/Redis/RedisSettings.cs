namespace admin_infrastructure.Models.Redis;

public sealed class RedisSettings
{
	public string? Configuration { get; set; }
	public string? InstanceName { get; set; }
	public int ConnectRetry { get; set; } = 3;
	public int ConnectTimeoutMs { get; set; } = 5000;
	public int MaxConnectAttempts { get; set; } = 5;
	public int InitialRetryDelayMs { get; set; } = 200;
}