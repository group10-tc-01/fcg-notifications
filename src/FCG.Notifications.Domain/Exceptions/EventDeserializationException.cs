using System.Net;

namespace FCG.Notifications.Domain.Exceptions
{
    public class EventDeserializationException : NotificationException
    {
        public EventDeserializationException(string message)
            : base(message, HttpStatusCode.BadRequest, "EVENT_DESERIALIZATION_ERROR")
        {
        }

        public EventDeserializationException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.BadRequest, "EVENT_DESERIALIZATION_ERROR")
        {
        }
    }
}
