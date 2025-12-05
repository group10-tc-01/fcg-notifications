namespace FCG.Notifications.Domain.Events
{
    public record UserCreatedEvent(string UserId, string Name, string Email, string CorrelationId, string OccuredAt);
}
