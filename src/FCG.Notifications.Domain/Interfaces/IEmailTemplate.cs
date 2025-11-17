namespace FCG.Notifications.Domain.Interfaces
{
    public interface IEmailTemplate
    {
        string GetSubject();

        string GetHtmlContent();
    }
}
