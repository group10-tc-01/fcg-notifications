using FCG.Notifications.Domain.Events;
using FCG.Notifications.Domain.Interfaces;
using FCG.Notifications.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.Notifications.Infrastructure.EventHandlers
{
    public class UserCreatedEventHandler : IKafkaEventHandler<UserCreatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(
            IEmailService emailService,
            ILogger<UserCreatedEventHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task HandleAsync(UserCreatedEvent eventData, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing UserCreatedEvent for {Email}", eventData.UserEmail);

            await _emailService.SendUserCreatedEmailAsync(
                eventData.UserName,
                eventData.UserEmail,
                cancellationToken);

            _logger.LogInformation("Successfully sent welcome email to {Email}", eventData.UserEmail);
        }
    }
}
