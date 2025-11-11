using FCG.Notifications.Domain.Events;
using FCG.Notifications.Domain.Interfaces;
using FCG.Notifications.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.Notifications.Infrastructure.EventHandlers
{
    public class PaymentProcessedEventHandler : IKafkaEventHandler<PaymentProcessedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<PaymentProcessedEventHandler> _logger;

        public PaymentProcessedEventHandler(
            IEmailService emailService,
            ILogger<PaymentProcessedEventHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task HandleAsync(PaymentProcessedEvent eventData, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing PaymentProcessedEvent for {Email}", eventData.UserEmail);

            await _emailService.SendPaymentProcessedEmailAsync(eventData.UserEmail, eventData.IsSuccessful, cancellationToken);

            _logger.LogInformation("Successfully sent payment processed email to {Email}", eventData.UserEmail);
        }
    }
}
