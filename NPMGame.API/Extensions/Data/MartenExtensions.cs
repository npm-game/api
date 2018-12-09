using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Models.Config;

namespace NPMGame.API.Extensions.Data
{
    public static class MartenExtensions
    {
        public static void AddMarten(this IServiceCollection services, IConfiguration config)
        {// configure strongly typed settings objects
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();
            var dbConfig = appSettings.Database;

            services.AddScoped<IDocumentStore>(provider =>
                DocumentStore.For(options =>
                {
                    options.Connection($"host={dbConfig.Host};port={dbConfig.Port};database={dbConfig.Name};username={dbConfig.Username};password={dbConfig.Password}");
                })
            );
        }
    }
}