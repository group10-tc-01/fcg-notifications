namespace FCG.Notifications.Infrastructure.Settings
{
    public record EmailSettings
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string SenderAddress { get; init; } = string.Empty;
    }
}
