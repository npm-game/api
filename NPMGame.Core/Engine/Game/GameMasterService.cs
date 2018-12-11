using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.Core.Base;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.Core.Engine.Game
{
    public class GameMasterService : BaseService
    {
        private readonly IServiceProvider _serviceProvider;

        public GameMasterService(IServiceProvider serviceProvider, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<GameSession> CreateGame(Guid creatorUserId, GameOptions options = null)
        {
            var creatorUser = await UnitOfWork.GetRepository<UserRepository>().Get(creatorUserId);

            if (creatorUser == null)
            {
                throw new GameException(ErrorMessages.UserNotFound);
            }

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

            return await ForGame(game, async (handler) =>
            {
                handler.AddPlayerToGame(creatorUser.Id);
            });
        }

        public async Task<GameSession> SaveGame(GameSession game)
        {
            return await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);
        }

        public async Task<GameSession> ForGame(Guid gameId, Func<IGameHandlerService, Task> handlerFunc)
        {
            var game = await UnitOfWork.GetRepository<GameSessionRepository>().Get(gameId);

            if (game == null)
            {
                throw new GameException(ErrorMessages.GameNotFound);
            }

            return await ForGame(game, handlerFunc);
        }

        public async Task<GameSession> ForGame(GameSession game, Func<IGameHandlerService, Task> handlerFunc)
        {
            var handler = _serviceProvider.GetRequiredService<IGameHandlerService>();

            await handlerFunc(handler.UsingGame(game));

            return await SaveGame(handler.GetGame());
        }
    }
}