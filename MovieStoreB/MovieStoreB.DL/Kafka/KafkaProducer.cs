using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Serialization;

namespace MovieStoreB.DL.Kafka
{
    internal class KafkaProducer<TKey, TData> : IKafkaProducer<TData>
        where TData : CacheItem<TKey>
        where TKey : notnull
    {
        private readonly ProducerConfig _config;
        private readonly IProducer<TKey, TData> _producer;
        private readonly ILogger<KafkaProducer<TKey, TData>> _logger;
        private readonly string _defaultTopic;

        public KafkaProducer(ILogger<KafkaProducer<TKey, TData>> logger)
        {
            _logger = logger;
            _defaultTopic = typeof(TData).Name.ToLower() + "_cache";

            _config = new ProducerConfig()
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false
            };

            _producer = new ProducerBuilder<TKey, TData>(_config)
                .SetValueSerializer(new MsgPackSerializer<TData>()) 
                .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Error}", e.Reason))
                .Build();
        }

        public async Task Produce(TData message)
        {
            try
            {
                await _producer.ProduceAsync(_defaultTopic, new Message<TKey, TData>
                {
                    Key = message.GetKey(),
                    Value = message
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