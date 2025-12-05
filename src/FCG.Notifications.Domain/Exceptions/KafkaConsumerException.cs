using System.Net;

namespace FCG.Notifications.Domain.Exceptions
{
    public class KafkaConsumerException : NotificationException
    {
        public KafkaConsumerException(string message)
            : base(message, HttpStatusCode.ServiceUnavailable, "KAFKA_CONSUMER_ERROR")
        {
        }

        public KafkaConsumerException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.ServiceUnavailable, "KAFKA_CONSUMER_ERROR")
        {
        }
    }
}
