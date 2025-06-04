using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieStoreB.DL.Cache;
using MovieStoreB.DL.Kafka;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.DL.Cache
{
    public class KafkaCacheDistributor<TData, TDataRepository, TConfigurationType, TKey> : BackgroundService
        where TDataRepository : ICacheRepository<TData>
        where TData : CacheItem<TKey>
        where TConfigurationType : CacheConfiguration
        where TKey : notnull
    {
        private readonly ICacheRepository<TData> _cacheRepository;
        private readonly IKafkaProducer<TData> _kafkaProducer;
        private readonly IOptionsMonitor<TConfigurationType> _configuration;
        private readonly ILogger<KafkaCacheDistributor<TData, TDataRepository, TConfigurationType, TKey>> _logger;

        public KafkaCacheDistributor(
            ICacheRepository<TData> cacheRepository,
            IKafkaProducer<TData> kafkaProducer,
            IOptionsMonitor<TConfigurationType> configuration,
            ILogger<KafkaCacheDistributor<TData, TDataRepository, TConfigurationType, TKey>> logger)
        {
            _cacheRepository = cacheRepository;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastExecuted = DateTime.UtcNow;

            _logger.LogInformation("Starting Kafka Cache Distributor for {DataType}", typeof(TData).Name);

            try
            {
                // Initial full load
                var initialData = await _cacheRepository.FullLoad();
                if (initialData?.Any() == true)
                {
                    await _kafkaProducer.ProduceBatches(initialData.Where(x => x != null)!);
                    _logger.LogInformation("Published {Count} initial {DataType} items to Kafka",
                        initialData.Count(), typeof(TData).Name);
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_configuration.CurrentValue.RefreshInterval), stoppingToken);

                        var updatedData = await _cacheRepository.DifLoad(lastExecuted);

                        if (updatedData?.Any() == true)
                        {
                            var validData = updatedData.Where(x => x != null).ToList();

                            if (validData.Any())
                            {
                                await _kafkaProducer.ProduceBatches(validData!);

                                var lastUpdated = validData.Max(x => x!.DateInserted);
                                lastExecuted = lastUpdated;

                                _logger.LogInformation("Published {Count} updated {DataType} items to Kafka",
                                    validData.Count, typeof(TData).Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in Kafka Cache Distributor iteration for {DataType}", typeof(TData).Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Kafka Cache Distributor for {DataType}", typeof(TData).Name);
            }
        }
    }
}