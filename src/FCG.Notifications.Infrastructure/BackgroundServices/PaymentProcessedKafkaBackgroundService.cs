using FCG.Notifications.Domain.Events;
using FCG.Notifications.Infrastructure.ExceptionHandlers;
using FCG.Notifications.Infrastructure.Services;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Infrastructure.BackgroundServices
{
    public class PaymentProcessedKafkaBackgroundService : KafkaBackgroundService<PaymentProcessedEvent>
    {
        public PaymentProcessedKafkaBackgroundService(
            KafkaConsumerFactory consumerFactory,
            IOptions<KafkaSettings> kafkaSettings,
            IServiceProvider serviceProvider,
            GlobalExceptionHandler exceptionHandler,
            ILogger<KafkaBackgroundService<PaymentProcessedEvent>> logger)
            : base(consumerFactory.CreateConsumer(kafkaSettings.Value.PaymentProcessedTopic, kafkaSettings.Value.PaymentProcessedGroupId), serviceProvider, exceptionHandler, logger, kafkaSettings)
        {
        }
    }
}
