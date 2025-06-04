using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieStoreB.DL.Cache;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Serialization;

namespace MovieStoreB.DL.Kafka
{
    public class KafkaCacheConsumer<TData, TKey, TConfigurationType> : BackgroundService, IKafkaConsumer<TData, TKey>
        where TData : CacheItem<TKey>
        where TKey : notnull
        where TConfigurationType : CacheConfiguration
    {
        private readonly ConsumerConfig _config;
        private readonly IConsumer<TKey, TData> _consumer;
        private readonly IInMemoryCacheService<TData, TKey> _cacheService;
        private readonly IOptionsMonitor<TConfigurationType> _configuration;
        private readonly ILogger<KafkaCacheConsumer<TData, TKey, TConfigurationType>> _logger;

        public KafkaCacheConsumer(
            IInMemoryCacheService<TData, TKey> cacheService,
            IOptionsMonitor<TConfigurationType> configuration,
            ILogger<KafkaCacheConsumer<TData, TKey, TConfigurationType>> logger)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _logger = logger;

            _config = new ConsumerConfig()
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false,
                GroupId = $"{typeof(TData).Name.ToLower()}_cache_consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<TKey, TData>(_config)
                .SetValueDeserializer(new MessagePackDeserializer<TData>())
                .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Error}", e.Reason))
                .Build();

            _consumer.Subscribe(_configuration.CurrentValue.Topic);
        }

        public async Task Consume(IEnumerable<TData> messages)
        {
            if (messages?.Any() == true)
            {
                await _cacheService.AddOrUpdateBatch(messages);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Kafka Cache Consumer for {DataType} on topic {Topic}",
                typeof(TData).Name, _configuration.CurrentValue.Topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));

                        if (consumeResult?.Message?.Value != null)
                        {
                            await _cacheService.AddOrUpdate(consumeResult.Message.Value);
                            _consumer.Commit(consumeResult);

                            _logger.LogDebug("Consumed and cached {DataType} with key {Key}",
                                typeof(TData).Name, consumeResult.Message.Key);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message from Kafka for {DataType}", typeof(TData).Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error in Kafka consumer for {DataType}", typeof(TData).Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Kafka Cache Consumer for {DataType}", typeof(TData).Name);
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
                _logger.LogInformation("Kafka Cache Consumer for {DataType} stopped", typeof(TData).Name);
            }
        }
    }
}