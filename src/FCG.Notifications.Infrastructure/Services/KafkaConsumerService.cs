using Confluent.Kafka;
using FCG.Notifications.Domain.Exceptions;
using FCG.Notifications.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FCG.Notifications.Infrastructure.Services
{
    public class KafkaConsumerService : IDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly string _topic;

        public KafkaConsumerService(
            IOptions<KafkaSettings> kafkaSettings,
            ILogger<KafkaConsumerService> logger,
            string topic)
        {
            _kafkaSettings = kafkaSettings.Value;
            _logger = logger;
            _topic = topic;

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = _kafkaSettings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    throw new KafkaConsumerException($"Kafka error: {error.Reason}");
                })
                .Build();

            _consumer.Subscribe(_topic);
            _logger.LogInformation("Kafka consumer subscribed to topic: {Topic}", _topic);
        }

        public ConsumeResult<string, string> ConsumeAsync(CancellationToken cancellationToken)
        {
            var consumeResult = _consumer.Consume(cancellationToken);

            if (consumeResult == null || consumeResult.IsPartitionEOF)
            {
                return null!;
            }

            _logger.LogInformation(
                "Consumed message from topic {Topic}, partition {Partition}, offset {Offset}",
                consumeResult.Topic,
                consumeResult.Partition.Value,
                consumeResult.Offset.Value);

            return consumeResult;
        }

        public void Commit(ConsumeResult<string, string> consumeResult)
        {
            _consumer.Commit(consumeResult);
            
            _logger.LogDebug("Committed offset {Offset} for partition {Partition}",
                consumeResult.Offset.Value,
                consumeResult.Partition.Value);
        }

        public T Deserialize<T>(string message)
        {
            var result = JsonSerializer.Deserialize<T>(message);

            if (result == null)
            {
                throw new EventDeserializationException($"Failed to deserialize message to type {typeof(T).Name}");
            }

            return result;
        }

        public void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
    }
}
