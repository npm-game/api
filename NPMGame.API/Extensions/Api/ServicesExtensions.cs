using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Repositories.Identity;
using NPMGame.Core.Extensions;

namespace NPMGame.API.Extensions.Api
{
    public static class ServicesExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<AuthRepository>();
        }
    }
}