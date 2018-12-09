using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Extensions.Data;

namespace NPMGame.API.Extensions
{
    public static class CoreExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddMarten(config);
            services.AddUnitOfWork();

            services.AddValidator();
        }
    }
}