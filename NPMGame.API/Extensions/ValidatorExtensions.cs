using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Services;

namespace NPMGame.API.Extensions
{
    public static class ValidatorExtensions
    {
        public static void AddValidator(this IServiceCollection services)
        {
            services.AddScoped<IValidatorService, ValidatorService>();
        }
    }
}