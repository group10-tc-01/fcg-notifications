namespace FCG.Notifications.Infrastructure.Interfaces
{
    public interface IKafkaEventHandler<TEvent>
    {
        Task HandleAsync(TEvent eventData, CancellationToken cancellationToken);
    }
}
