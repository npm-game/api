using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Repositories.Identity;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.API.Extensions.Data
{
    public static class UnitOfWorkExtensions
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            // base unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<AuthRepository>();

            services.AddScoped<GameSessionRepository>();
        }
    }
}