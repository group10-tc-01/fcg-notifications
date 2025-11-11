using Azure;
using Azure.Communication.Email;
using FCG.Notifications.Domain.Exceptions;
using FCG.Notifications.Domain.Interfaces;
using FCG.Notifications.Infrastructure.EmailTemplates;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Notifications.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _emailClient = new EmailClient(_emailSettings.ConnectionString);
        }

        public async Task SendUserCreatedEmailAsync(string userName, string userEmail, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending welcome email to {Email}", userEmail);

            var template = new WelcomeEmailTemplate(userName);

            await SendEmailAsync(userEmail, template, cancellationToken);

            _logger.LogInformation("Welcome email sent successfully to {Email}", userEmail);
        }

        private async Task SendEmailAsync(string recipientEmail, IEmailTemplate template, CancellationToken cancellationToken)
        {
            var emailMessage = new EmailMessage(
                senderAddress: _emailSettings.SenderAddress,
                recipientAddress: recipientEmail,
                content: new EmailContent(template.GetSubject())
                {
                    Html = template.GetHtmlContent()
                });

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage,
                cancellationToken);

            if (emailSendOperation.Value.Status != EmailSendStatus.Succeeded)
            {
                throw new EmailServiceException($"Failed to send email to {recipientEmail}. Status: {emailSendOperation.Value.Status}");
            }

            _logger.LogDebug("Email sent successfully to {Email}. Status: {Status}",
                recipientEmail,
                emailSendOperation.Value.Status);
        }
    }
}
