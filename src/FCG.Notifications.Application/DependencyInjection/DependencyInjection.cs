using FCG.Notifications.Application.Common.Abstractions;
using FCG.Notifications.Application.Common.ExceptionHandlers;
using FCG.Notifications.Application.Common.Services;
using FCG.Notifications.Application.Common.Settings;
using FCG.Notifications.Application.Features.PaymentProcessed;
using FCG.Notifications.Application.Features.UserCreated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FCG.Notifications.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));

            services.AddSingleton<GlobalExceptionHandler>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddHostedService<UserCreatedConsumer>();
            services.AddHostedService<PaymentProcessedConsumer>();

            services.AddSerilogLogging(configuration);

            return services;
        }

        private static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Seq(configuration["Serilog:SeqUrl"] ?? "http://localhost:5341")
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });
        }
    }
}
