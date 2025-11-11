using System.Net;

namespace FCG.Notifications.Domain.Exceptions
{
    public abstract class NotificationException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        protected NotificationException(string message, HttpStatusCode statusCode, string errorCode)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        protected NotificationException(string message, Exception innerException, HttpStatusCode statusCode, string errorCode)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
