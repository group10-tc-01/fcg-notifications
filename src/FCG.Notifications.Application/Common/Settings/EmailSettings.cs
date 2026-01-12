namespace FCG.Notifications.Application.Common.Settings
{
    public record EmailSettings
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string SenderAddress { get; init; } = string.Empty;
    }
}
