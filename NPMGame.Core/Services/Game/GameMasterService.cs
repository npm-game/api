using System;
using System.Threading.Tasks;
using NPMGame.Core.Base;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Repositories.Game;

namespace NPMGame.Core.Services.Game
{
    public class GameMasterService : BaseService
    {
        private readonly GameHandlerService _handler;

        public GameMasterService(GameHandlerService handler, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _handler = handler;
        }

        public async Task<GameSession> CreateGame(Guid creatorUserId, GameOptions options = null)
        {
            if (options == null)
            {
                options = new GameOptions
                {
                    Goal = 100,
                    HandSize = 7
                };
            }

            var game = new GameSession(options);

            game = await UnitOfWork.GetRepository<GameSessionRepository>().Create(game);

            game = await _handler.UsingGame(game).AddPlayerToGame(creatorUserId);

            return game;
        }

        public async Task<GameHandlerService> ForGame(Guid gameId)
        {
            var game = await UnitOfWork.GetRepository<GameSessionRepository>().Get(gameId);

            if (game == null)
            {
                throw new GameException(ErrorMessages.GameNotFound);
            }

            return _handler.UsingGame(game);
        }
    }
}