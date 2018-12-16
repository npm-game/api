using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.Core.Base;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Models.Identity;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.Core.Engine.Game
{
    public interface IGameMasterService
    {
        Task<GameSession> CreateGame(User creatorUser, GameOptions options = null);
        Task<GameSession> SaveGame(GameSession game);
        Task<IGameHandlerService> CreateHandler(Guid gameId);
        IGameHandlerService CreateHandler(GameSession game);
    }

    public class GameMasterService : BaseService, IGameMasterService
    {
        private readonly IServiceProvider _serviceProvider;

        public GameMasterService(IServiceProvider serviceProvider, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<GameSession> CreateGame(User creatorUser, GameOptions options = null)
        {
            if (options == null)
            {
                options = new GameOptions
                {
                    Goal = 1000,
                    HandSize = 7
                };
            }

            var game = new GameSession
            {
                Options = options
            };

            game = await UnitOfWork.GetRepository<GameSessionRepository>().Create(game);

            game = CreateHandler(game)
                .AddPlayerToGame(creatorUser.Id)
                .GetGame();

            await SaveGame(game);

            return game;
        }

        public async Task<GameSession> SaveGame(GameSession game)
        {
            return await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);
        }

        public async Task<IGameHandlerService> CreateHandler(Guid gameId)
        {
            var game = await UnitOfWork.GetRepository<GameSessionRepository>().Get(gameId);

            if (game == null)
            {
                throw new GameException(ErrorMessages.GameNotFound);
            }

            return CreateHandler(game);
        }

        public IGameHandlerService CreateHandler(GameSession game)
        {
            var handler = _serviceProvider.GetRequiredService<IGameHandlerService>();

            return handler.UsingGame(game);
        }
    }
}