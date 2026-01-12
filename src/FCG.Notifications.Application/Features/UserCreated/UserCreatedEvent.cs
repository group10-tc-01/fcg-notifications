namespace FCG.Notifications.Application.Features.UserCreated
{
    public record UserCreatedEvent(string UserId, string Name, string Email, string CorrelationId, string CreatedAt);
}
