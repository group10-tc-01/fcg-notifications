namespace FCG.Notifications.Application.Common.Settings
{
    public sealed class KafkaSettings
    {
        public string BootstrapServers { get; init; } = string.Empty;
        public string GroupId { get; init; } = string.Empty;
        public int ConsumerTimeoutMs { get; init; } = 100;
        public KafkaTopics Topics { get; init; } = new();
    }

    public sealed class KafkaTopics
    {
        public string UserCreated { get; init; } = string.Empty;
        public string PaymentProcessed { get; init; } = string.Empty;
    }
}
