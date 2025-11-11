namespace FCG.Notifications.Domain.Events
{
    public record UserCreatedEvent(string UserName, string UserEmail);
}
