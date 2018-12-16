using Microsoft.Extensions.DependencyInjection;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.Core.Extensions
{
    public static class CoreExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddUnitOfWork();
            services.AddValidator();

            services.AddServices();
            services.AddRepositories();
        }

        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddValidator(this IServiceCollection services)
        {
            services.AddScoped<IValidatorService, ValidatorService>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ILetterGeneratorService, LetterGeneratorService>();
            services.AddScoped<IWordMatchingService, WordMatchingService>();
            services.AddScoped<IWordScoringService, WordScoringService>();

            services.AddScoped<IGameMasterService, GameMasterService>();
            services.AddScoped<IGameHandlerService, GameHandlerService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<UserRepository>();
            services.AddScoped<GameSessionRepository>();
        }
    }
}