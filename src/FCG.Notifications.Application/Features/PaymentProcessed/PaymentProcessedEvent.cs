namespace FCG.Notifications.Application.Features.PaymentProcessed
{
    public record PaymentProcessedEvent(string UserEmail, Guid CorrelationId, Guid PaymentId, Guid UserId, Guid GameId, decimal Amount, string Status, DateTime ProcessedAt);
}
