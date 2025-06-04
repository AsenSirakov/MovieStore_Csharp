using Microsoft.Extensions.DependencyInjection;
using MovieStoreB.BL.Interfaces;
using MovieStoreB.BL.Services;

namespace MovieStoreB.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            // Register all your services
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IActorService, ActorService>();
            services.AddSingleton<IBlMovieService, BlMovieService>();

            // Register enhanced movie service separately (not as IMovieService)
            services.AddSingleton<EnhancedMovieService>();

            return services;
        }
    }
}