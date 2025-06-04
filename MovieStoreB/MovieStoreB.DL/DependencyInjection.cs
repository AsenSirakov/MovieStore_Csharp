using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieStoreB.DL.Cache;
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
            // Register repositories
            services.AddSingleton<IMovieRepository, MoviesRepository>();
            services.AddSingleton<IActorRepository, ActorMongoRepository>();

<<<<<<< Updated upstream
            //services.AddHostedService<MongoCacheDistributor>();
            //services.AddSingleton<ICacheRepository<Movie>, MoviesRepository>();

            services.AddCache<MoviesCacheConfiguration, MoviesRepository, Movie, string>(config);
            services.AddCache<ActorsCacheConfiguration, ActorMongoRepository, Actor, string>(config);

            //services.AddHostedService<MongoCachePopulator<Movie, IMovieRepository>>();
=======
            // Register cache services
            services.AddCache<MoviesCacheConfiguration, MoviesRepository, Movie, string>(config);
            services.AddCache<ActorsCacheConfiguration, ActorMongoRepository, Actor, string>(config);

            // Register in-memory cache services
            services.AddSingleton<IInMemoryCacheService<Movie, string>, InMemoryCacheService<Movie, string>>();
            services.AddSingleton<IInMemoryCacheService<Actor, string>, InMemoryCacheService<Actor, string>>();
>>>>>>> Stashed changes

            return services;
        }

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
<<<<<<< Updated upstream
            services.AddHostedService<MongoCachePopulator<TData, ICacheRepository<TData>, TCacheConfiguration, TKey>>();
=======

            // Register Kafka Cache Distributor (publishes to Kafka)
            services.AddHostedService<KafkaCacheDistributor<TData, ICacheRepository<TData>, TCacheConfiguration, TKey>>();

            // Register Kafka Cache Consumer (reads from Kafka and updates in-memory cache)
            services.AddHostedService<KafkaCacheConsumer<TData, TKey, TCacheConfiguration>>();
>>>>>>> Stashed changes


            return services;
        }
    }

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
