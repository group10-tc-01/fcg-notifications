namespace FCG.Notifications.Infrastructure.Settings
{
    public record KafkaSettings
    {
        public string BootstrapServers { get; init; } = string.Empty;
        public string UserCreatedTopic { get; init; } = string.Empty;
        public string GroupId { get; init; } = string.Empty;
    }
}
