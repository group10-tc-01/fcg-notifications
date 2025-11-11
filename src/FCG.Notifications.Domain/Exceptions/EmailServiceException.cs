using System.Net;

namespace FCG.Notifications.Domain.Exceptions
{
    public class EmailServiceException : NotificationException
    {
        public EmailServiceException(string message)
            : base(message, HttpStatusCode.ServiceUnavailable, "EMAIL_SERVICE_ERROR")
        {
        }

        public EmailServiceException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.ServiceUnavailable, "EMAIL_SERVICE_ERROR")
        {
        }
    }
}
