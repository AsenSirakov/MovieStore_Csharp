using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieStoreB.DL.Cache;
using MovieStoreB.DL.Gateways;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.DL.Kafka;
using MovieStoreB.DL.Repositories.MongoRepositories;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.DL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration config)
        {
            // Register HttpClient for RestSharp
            services.AddHttpClient();

            // Your existing repository registrations
            services.AddSingleton<IMovieRepository, MoviesRepository>();
            services.AddSingleton<IActorRepository, ActorMongoRepository>();

            // Your existing in-memory cache services
            services.AddSingleton<IInMemoryCacheService<Movie, string>, InMemoryCacheService<Movie, string>>();
            services.AddSingleton<IInMemoryCacheService<Actor, string>, InMemoryCacheService<Actor, string>>();

            // Register the ActorBioGateway
            services.AddSingleton<IActorBioGateway, ActorBioGateway>();

            // Check if Kafka is enabled before registering Kafka services
            var kafkaEnabled = config.GetValue<bool>("KafkaConfiguration:Enabled");

            if (kafkaEnabled)
            {
                // Only register Kafka services if enabled
                services.AddCache<MoviesCacheConfiguration, MoviesRepository, Movie, string>(config);
                services.AddCache<ActorsCacheConfiguration, ActorMongoRepository, Actor, string>(config);
            }
            else
            {
                // Register cache configurations without Kafka services
                services.Configure<MoviesCacheConfiguration>(config.GetSection(nameof(MoviesCacheConfiguration)));
                services.Configure<ActorsCacheConfiguration>(config.GetSection(nameof(ActorsCacheConfiguration)));

                // Register cache repositories only (no Kafka)
                services.AddSingleton<ICacheRepository<Movie>, MoviesRepository>();
                services.AddSingleton<ICacheRepository<Actor>, ActorMongoRepository>();
            }

            return services;
        }

        // Modified AddCache method to only register when Kafka is enabled
        public static IServiceCollection AddCache<TCacheConfiguration, TCacheRepository, TData, TKey>(
            this IServiceCollection services, IConfiguration config)
            where TCacheConfiguration : CacheConfiguration
            where TCacheRepository : class, ICacheRepository<TData>
            where TData : CacheItem<TKey>
            where TKey : notnull
        {
            var configSection = config.GetSection(typeof(TCacheConfiguration).Name);

            if (!configSection.Exists())
            {
                throw new ArgumentNullException(typeof(TCacheConfiguration).Name, "Configuration section is missing in appsettings!");
            }

            services.Configure<TCacheConfiguration>(configSection);

            // Register repositories and producers
            services.AddSingleton<ICacheRepository<TData>, TCacheRepository>();
            services.AddSingleton<IKafkaProducer<TData>, KafkaProducer<TKey, TData>>();

            // Register Kafka Cache Distributor (publishes to Kafka)
            services.AddHostedService<KafkaCacheDistributor<TData, ICacheRepository<TData>, TCacheConfiguration, TKey>>();

            // Register Kafka Cache Consumer (reads from Kafka and updates in-memory cache)
            services.AddHostedService<KafkaCacheConsumer<TData, TKey, TCacheConfiguration>>();

            return services;
        }
    }

    // Your existing configurations
    public class MoviesCacheConfiguration : CacheConfiguration
    {
    }

    public class ActorsCacheConfiguration : CacheConfiguration
    {
    }

    public class CacheConfiguration
    {
        public string Topic { get; set; } = string.Empty;
        public int RefreshInterval { get; set; } = 30;
    }
}