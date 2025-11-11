using FCG.Notifications.Domain.Events;
using FCG.Notifications.Domain.Interfaces;
using FCG.Notifications.Infrastructure.BackgroundServices;
using FCG.Notifications.Infrastructure.EventHandlers;
using FCG.Notifications.Infrastructure.ExceptionHandlers;
using FCG.Notifications.Infrastructure.Interfaces;
using FCG.Notifications.Infrastructure.Services;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Notifications.Infrastructure.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));

            services.AddSingleton<KafkaConsumerFactory>();

            services.AddSingleton<GlobalExceptionHandler>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IKafkaEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

            services.AddHostedService<UserCreatedKafkaBackgroundService>();

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
