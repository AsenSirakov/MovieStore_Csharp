using Microsoft.Extensions.DependencyInjection;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.BL.Services;

namespace MovieStoreB.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            // Register services
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IActorService, ActorService>();
            services.AddSingleton<IBlMovieService, BlMovieService>();

            // Register enhanced movie service separately (not as IMovieService)
            services.AddSingleton<EnhancedMovieService>();

            // Register external API service with HttpClient - FIXED
            services.AddHttpClient<IExternalApiService, ExternalApiService>();

            return services;
        }
    }
}