using FCG.Notifications.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Notifications.Infrastructure.ExceptionHandlers
{
    [ExcludeFromCodeCoverage]
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void HandleException(Exception exception, string context)
        {
            var exceptionId = Guid.NewGuid().ToString();

            if (exception is NotificationException notificationException)
            {
                HandleNotificationException(notificationException, context, exceptionId);
                return;
            }

            HandleGenericException(exception, context, exceptionId);
        }

        private void HandleNotificationException(NotificationException exception, string context, string exceptionId)
        {
            _logger.LogError(
                exception,
                "[{ExceptionId}] [{ErrorCode}] Notification exception in {Context}: {Message}. StatusCode: {StatusCode}",
                exceptionId,
                exception.ErrorCode,
                context,
                exception.Message,
                exception.StatusCode);
        }

        private void HandleGenericException(Exception exception, string context, string exceptionId)
        {
            _logger.LogCritical(
                exception,
                "[{ExceptionId}] Unhandled exception in {Context}: {Message}. StackTrace: {StackTrace}",
                exceptionId,
                context,
                exception.Message,
                exception.StackTrace);
        }

        public async Task<bool> TryExecuteAsync(Func<Task> action, string context, bool continueOnError = false)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, context);

                if (!continueOnError)
                {
                    throw;
                }

                return false;
            }
        }

        public bool ShouldCommitMessage(Exception exception)
        {
            // Commit messages that failed due to deserialization issues (poison messages)
            // Don't commit messages that failed due to transient errors (email service, etc)
            return exception is EventDeserializationException;
        }
    }
}
