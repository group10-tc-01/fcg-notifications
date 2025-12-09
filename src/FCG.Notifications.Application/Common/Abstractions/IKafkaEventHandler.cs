namespace FCG.Notifications.Application.Common.Abstractions
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}
