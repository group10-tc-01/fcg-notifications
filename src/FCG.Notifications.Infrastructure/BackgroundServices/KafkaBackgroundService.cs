using FCG.Notifications.Infrastructure.ExceptionHandlers;
using FCG.Notifications.Infrastructure.Interfaces;
using FCG.Notifications.Infrastructure.Services;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Infrastructure.BackgroundServices
{
    public class KafkaBackgroundService<TEvent> : BackgroundService where TEvent : class
    {
        private readonly KafkaConsumerService _kafkaConsumerService;
        private readonly IServiceProvider _serviceProvider;
        private readonly GlobalExceptionHandler _exceptionHandler;
        private readonly ILogger<KafkaBackgroundService<TEvent>> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly string _eventTypeName;

        public KafkaBackgroundService(
            KafkaConsumerService kafkaConsumerService,
            IServiceProvider serviceProvider,
            GlobalExceptionHandler exceptionHandler,
            ILogger<KafkaBackgroundService<TEvent>> logger,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaConsumerService = kafkaConsumerService;
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
            _logger = logger;
            _kafkaSettings = kafkaSettings.Value;
            _eventTypeName = typeof(TEvent).Name;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Background Service started for event type: {EventType}", _eventTypeName);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = await _kafkaConsumerService.ConsumeAsync(stoppingToken);

                    if (consumeResult == null)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(GetPollingDelayMs()), stoppingToken);
                        continue;
                    }

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
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka Background Service for {EventType}", _eventTypeName);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("Kafka Background Service stopping for {EventType}", _eventTypeName);
        }

        private int GetPollingDelayMs()
        {
            return _kafkaSettings.PollingDelayMs;
        }

        public override void Dispose()
        {
            _kafkaConsumerService?.Dispose();
            base.Dispose();
        }
    }
}
