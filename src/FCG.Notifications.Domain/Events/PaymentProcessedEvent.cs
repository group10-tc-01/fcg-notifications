namespace FCG.Notifications.Domain.Events
{
    public record PaymentProcessedEvent(string UserEmail, bool IsSuccessful);
}
