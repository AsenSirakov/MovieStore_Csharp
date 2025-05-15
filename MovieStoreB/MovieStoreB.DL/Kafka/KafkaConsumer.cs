using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Serialization;

namespace MovieStoreB.DL.Kafka
{
    internal class KafkaConsumer<TData, TKey> : BackgroundService, IKafkaConsumer<TData, TKey>
        where TData : CacheItem<TKey>
        where TKey : notnull
    {
        private readonly ConsumerConfig _config;
        private readonly IConsumer<TKey, TData> _consumer;
        private readonly ILogger<KafkaConsumer<TData, TKey>> _logger;
        private readonly string _topicName;

        public KafkaConsumer(ILogger<KafkaConsumer<TData, TKey>> logger)
        {
            _logger = logger;

            // Set topic based on data type
            _topicName = typeof(TData).Name == "Movie" ? "movies_cache" : "actors_cache";

            _config = new ConsumerConfig()
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                GroupId = $"KafkaChat_{typeof(TData).Name}_{Guid.NewGuid()}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false
            };

            _consumer = new ConsumerBuilder<TKey, TData>(_config)
                .SetValueDeserializer(new MessagePackDeserializer<TData>())
                .Build();
            _consumer.Subscribe(_topicName);

            _logger.LogInformation($"Consumer subscribed to '{_topicName}' topic");
        }

        public async Task Consume(IEnumerable<TData> messages)
        {
            foreach (var message in messages)
            {
                _logger.LogInformation($"Processing message: {message.GetKey()}");
                await Task.CompletedTask;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting Kafka consumer for topic: {_topicName}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation($"Consumed message from {_topicName}: {consumeResult.Message.Value.GetKey()}");
                        await Task.CompletedTask;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error consuming from {_topicName}");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        public override void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
            base.Dispose();
        }
    }
}