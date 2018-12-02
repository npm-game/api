using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Repositories.Identity;
using NPMGame.API.Services;

namespace NPMGame.API.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            // base unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // resources
            services.AddScoped<AuthRepository>();
        }
    }
}