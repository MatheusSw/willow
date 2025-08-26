namespace admin_application.Interfaces;

public interface IEventPublisher
{
	Task PublishAsync<T>(string channel, T eventData, CancellationToken cancellationToken = default) where T : class;
}
