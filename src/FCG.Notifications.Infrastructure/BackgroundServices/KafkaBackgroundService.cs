using FCG.Notifications.Infrastructure.ExceptionHandlers;
using FCG.Notifications.Infrastructure.Interfaces;
using FCG.Notifications.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FCG.Notifications.Infrastructure.BackgroundServices
{
    public class KafkaBackgroundService<TEvent> : BackgroundService where TEvent : class
    {
        private readonly KafkaConsumerService _kafkaConsumerService;
        private readonly IServiceProvider _serviceProvider;
        private readonly GlobalExceptionHandler _exceptionHandler;
        private readonly ILogger<KafkaBackgroundService<TEvent>> _logger;
        private readonly string _eventTypeName;

        public KafkaBackgroundService(
            KafkaConsumerService kafkaConsumerService,
            IServiceProvider serviceProvider,
            GlobalExceptionHandler exceptionHandler,
            ILogger<KafkaBackgroundService<TEvent>> logger)
        {
            _kafkaConsumerService = kafkaConsumerService;
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
            _logger = logger;
            _eventTypeName = typeof(TEvent).Name;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Background Service started for event type: {EventType}", _eventTypeName);

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _kafkaConsumerService.ConsumeAsync(stoppingToken);

                if (consumeResult == null)
                    continue;

                var success = await _exceptionHandler.TryExecuteAsync(async () =>
                {
                    var eventData = _kafkaConsumerService.Deserialize<TEvent>(consumeResult.Message.Value);

                    _logger.LogInformation("Processing {EventType}", _eventTypeName);

                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IKafkaEventHandler<TEvent>>();
                    await handler.HandleAsync(eventData, stoppingToken);

                }, $"Processing {_eventTypeName}", continueOnError: true);

                if (success)
                {
                    _kafkaConsumerService.Commit(consumeResult);
                    _logger.LogInformation("Successfully processed and committed {EventType}", _eventTypeName);
                }

                else
                {
                    _logger.LogWarning("Failed to process {EventType}. Message will be retried.", _eventTypeName);
                }
            }

            _logger.LogInformation("Kafka Background Service stopping for {EventType}", _eventTypeName);
        }

        public override void Dispose()
        {
            _kafkaConsumerService?.Dispose();
            base.Dispose();
        }
    }
}
