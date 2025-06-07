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

            services.AddSingleton<IMovieService, EnhancedMovieService>();

            // Register other services
            services.AddSingleton<IActorService, ActorService>();
            services.AddSingleton<IBlMovieService, BlMovieService>();

            // Register ExternalApiService
            services.AddSingleton<IExternalApiService, ExternalApiService>();

            // Register ActorBioGateway
            services.AddSingleton<IActorBioGateway, ActorBioGateway>();

        

            return services;
        }
    }
}