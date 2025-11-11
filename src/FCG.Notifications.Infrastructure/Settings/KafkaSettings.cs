namespace FCG.Notifications.Infrastructure.Settings
{
    public record KafkaSettings
    {
        public string BootstrapServers { get; init; } = string.Empty;
        public string UserCreatedTopic { get; init; } = string.Empty;
        public string PaymentProcessedTopic { get; init; } = string.Empty;
        public string UserCreatedGroupId { get; init; } = string.Empty;
        public string PaymentProcessedGroupId { get; init; } = string.Empty;
        public int ConsumerTimeoutMs { get; init; } = 1000;
        public int PollingDelayMs { get; init; } = 100;
    }
}
