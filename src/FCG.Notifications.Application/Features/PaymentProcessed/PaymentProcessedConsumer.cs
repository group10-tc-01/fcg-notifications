using FCG.Notifications.Application.Common.Abstractions;
using FCG.Notifications.Application.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Application.Features.PaymentProcessed
{
    public sealed class PaymentProcessedConsumer : BaseKafkaConsumer<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentProcessedConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PaymentProcessedConsumer(ILogger<PaymentProcessedConsumer> logger, IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider)
            : base(logger, kafkaSettings.Value.BootstrapServers, kafkaSettings.Value.GroupId, kafkaSettings.Value.Topics.PaymentProcessed, kafkaSettings.Value.ConsumerTimeoutMs)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ProcessEventAsync(PaymentProcessedEvent @event, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                _logger.LogInformation("Processing PaymentProcessedEvent for {Email}", @event.UserEmail);

                await emailService.SendPaymentProcessedEmailAsync(@event.UserEmail, @event.IsSuccessful, cancellationToken);

                _logger.LogInformation("Successfully sent payment processed email to {Email}", @event.UserEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PaymentProcessedEvent");
                throw;
            }
        }
    }
}
