using FCG.Notifications.Domain.Events;
using FCG.Notifications.Infrastructure.ExceptionHandlers;
using FCG.Notifications.Infrastructure.Services;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Infrastructure.BackgroundServices
{
    public class UserCreatedKafkaBackgroundService : KafkaBackgroundService<UserCreatedEvent>
    {
        public UserCreatedKafkaBackgroundService(
            KafkaConsumerFactory consumerFactory,
            IOptions<KafkaSettings> kafkaSettings,
            IServiceProvider serviceProvider,
            GlobalExceptionHandler exceptionHandler,
            ILogger<KafkaBackgroundService<UserCreatedEvent>> logger)
            : base(consumerFactory.CreateConsumer(kafkaSettings.Value.UserCreatedTopic, kafkaSettings.Value.UserCreatedGroupId), serviceProvider, exceptionHandler, logger, kafkaSettings)
        {
        }
    }
}
