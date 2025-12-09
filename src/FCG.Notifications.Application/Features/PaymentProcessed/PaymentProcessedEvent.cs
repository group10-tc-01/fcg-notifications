namespace FCG.Notifications.Application.Features.PaymentProcessed
{
    public record PaymentProcessedEvent(string UserEmail, bool IsSuccessful);
}
