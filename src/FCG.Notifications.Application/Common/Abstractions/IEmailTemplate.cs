namespace FCG.Notifications.Application.Common.Abstractions
{
    public interface IEmailTemplate
    {
        string GetSubject();
        string GetHtmlContent();
    }
}
