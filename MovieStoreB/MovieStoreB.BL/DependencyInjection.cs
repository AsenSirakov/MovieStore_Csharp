using Microsoft.Extensions.DependencyInjection;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.BL.Services;
using MovieStoreB.DL.Interfaces;
using MovieStoreB.DL.Gateways;

namespace MovieStoreB.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            // Register HttpClient for ExternalApiService
            services.AddHttpClient<ExternalApiService>();

            // Register all your services
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IActorService, ActorService>();
            services.AddSingleton<IBlMovieService, BlMovieService>();

            // Register ExternalApiService
            services.AddSingleton<IExternalApiService, ExternalApiService>();

            // Register ActorBioGateway - THIS WAS MISSING!
            services.AddSingleton<IActorBioGateway, ActorBioGateway>();

            // Register enhanced movie service separately
            services.AddSingleton<EnhancedMovieService>();

            return services;
        }
    }
}