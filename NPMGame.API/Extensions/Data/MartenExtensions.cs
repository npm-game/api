using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Models.Config;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Models.Identity;

namespace NPMGame.API.Extensions.Data
{
    public static class MartenExtensions
    {
        public static void AddMarten(this IServiceCollection services)
        {
            services.AddScoped<IDocumentStore>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                var appSettings = config.GetSection("AppSettings").Get<AppSettings>();
                var dbConfig = appSettings.Database;

                return DocumentStore.For(options =>
                {
                    options.Connection($"host={dbConfig.Host};port={dbConfig.Port};database={dbConfig.Name};username={dbConfig.Username};password={dbConfig.Password}");

                    options.Serializer(new JsonNetSerializer
                    {
                        EnumStorage = EnumStorage.AsString,
                        Casing = Casing.Default
                    });
                });
            });
        }
    }
}