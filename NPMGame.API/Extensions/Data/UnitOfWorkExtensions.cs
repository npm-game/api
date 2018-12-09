using Microsoft.Extensions.DependencyInjection;
using NPMGame.Core.Services;

namespace NPMGame.API.Extensions.Data
{
    public static class UnitOfWorkExtensions
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            // base unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}