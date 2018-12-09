using Microsoft.Extensions.DependencyInjection;
using NPMGame.Core.Services;

namespace NPMGame.API.Extensions.Data
{
    public static class ValidatorExtensions
    {
        public static void AddValidator(this IServiceCollection services)
        {
            services.AddScoped<IValidatorService, ValidatorService>();
        }
    }
}