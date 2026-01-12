using FCG.Notifications.Application.Common.Abstractions;
using FCG.Notifications.Application.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Application.Features.UserCreated
{
    public sealed class UserCreatedConsumer : BaseKafkaConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger, IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider)
            : base(logger, kafkaSettings.Value.BootstrapServers, kafkaSettings.Value.GroupId, kafkaSettings.Value.Topics.UserCreated, kafkaSettings.Value.ConsumerTimeoutMs)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ProcessEventAsync(UserCreatedEvent @event, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                _logger.LogInformation("Processing UserCreatedEvent for {Email}", @event.Email);

                await emailService.SendUserCreatedEmailAsync(
                    @event.Name,
                    @event.Email,
                    cancellationToken);

                _logger.LogInformation("Successfully sent welcome email to {Email}", @event.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserCreatedEvent. CorrelationId: {CorrelationId}", @event.CorrelationId);
                throw;
            }
        }
    }
}
