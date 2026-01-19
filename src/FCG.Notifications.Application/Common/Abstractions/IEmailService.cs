namespace FCG.Notifications.Application.Common.Abstractions
{
    public interface IEmailService
    {
        Task SendUserCreatedEmailAsync(string userName, string userEmail, CancellationToken cancellationToken = default);
        Task SendPaymentProcessedEmailAsync(string userEmail, string status, CancellationToken cancellationToken = default);
    }
}
