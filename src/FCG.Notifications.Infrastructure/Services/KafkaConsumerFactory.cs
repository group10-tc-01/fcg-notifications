using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Infrastructure.Services
{
    public class KafkaConsumerFactory
    {
        private readonly IOptions<KafkaSettings> _kafkaSettings;
        private readonly ILoggerFactory _loggerFactory;

        public KafkaConsumerFactory(
            IOptions<KafkaSettings> kafkaSettings,
            ILoggerFactory loggerFactory)
        {
            _kafkaSettings = kafkaSettings;
            _loggerFactory = loggerFactory;
        }

        public KafkaConsumerService CreateConsumer(string topic, string groupId)
        {
            var logger = _loggerFactory.CreateLogger<KafkaConsumerService>();
            return new KafkaConsumerService(_kafkaSettings, logger, topic, groupId);
        }
    }
}
