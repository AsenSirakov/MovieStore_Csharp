// Update your KafkaProducer.cs - replace the entire file content:

using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using MovieStoreB.Models.DTO;
using System.Text.Json;

namespace MovieStoreB.DL.Kafka
{
    internal class KafkaProducer<TKey, TData> : IKafkaProducer<TData>
        where TData : CacheItem<TKey>
        where TKey : notnull
    {
        private readonly ProducerConfig _config;
        private readonly IProducer<TKey, string> _producer; // Changed to string value
        private readonly ILogger<KafkaProducer<TKey, TData>> _logger;
        private readonly string _defaultTopic;
        private readonly JsonSerializerOptions _jsonOptions;

        public KafkaProducer(ILogger<KafkaProducer<TKey, TData>> logger)
        {
            _logger = logger;
            _defaultTopic = typeof(TData).Name.ToLower() + "_cache";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            _config = new ProducerConfig()
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false
            };

            _producer = new ProducerBuilder<TKey, string>(_config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Error}", e.Reason))
                .Build();
        }

        public async Task Produce(TData message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message, _jsonOptions);

                await _producer.ProduceAsync(_defaultTopic, new Message<TKey, string>
                {
                    Key = message.GetKey(),
                    Value = json
                });

                _logger.LogDebug("Successfully produced message to topic {Topic}", _defaultTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error producing message to topic {Topic}", _defaultTopic);
                throw;
            }
        }

        public async Task ProduceAll(IEnumerable<TData> messages)
        {
            var tasks = messages.Select(message => Produce(message));
            await Task.WhenAll(tasks);
        }

        public async Task ProduceBatches(IEnumerable<TData> messages)
        {
            const int batchSize = 50;
            var batch = new List<Task>();

            foreach (var message in messages)
            {
                batch.Add(Produce(message));

                if (batch.Count == batchSize)
                {
                    await Task.WhenAll(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await Task.WhenAll(batch);
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}