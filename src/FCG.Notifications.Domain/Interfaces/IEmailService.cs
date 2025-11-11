namespace FCG.Notifications.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendUserCreatedEmailAsync(string userName, string userEmail, CancellationToken cancellationToken = default);
    }
}
