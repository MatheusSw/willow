namespace admin_application.Interfaces;

public interface ICacheInvalidator
{
	Task InvalidateAsync(string key, CancellationToken cancellationToken = default);
}
