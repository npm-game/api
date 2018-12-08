using System;
using System.Threading.Tasks;
using Marten;
using NPMGame.Core.Base;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;

namespace NPMGame.Core.Services.Game
{
    public class GameMasterService : BaseService
    {
        public GameMasterService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<GameSession> CreateGame(Guid creatorUserId, GameOptions options = null)
        {
            // TODO: Create GameSession factory to handle creator and default options and all that
            var game = new GameSession(options);

            var createdGame = await UnitOfWork.GetRepository<GameSessionRepository>().Create(game);

            return createdGame;
        }

        public async Task<GameSession> AddPlayerToGame(Guid gameId, Guid userId)
        {
            // TODO: Check if player already part of any other game
            var game = await UnitOfWork.GetRepository<GameSessionRepository>().Get(gameId);

            if (game == null)
            {
                throw new GameException(ErrorMessages.GameNotFound);
            }

            var user = await UnitOfWork.GetRepository<UserRepository>().Get(userId);

            if (user == null)
            {
                throw new GameException(ErrorMessages.UserNotFound);
            }

            // TODO: Create GamePlayer factory to handle user ties and all that
            var player = new GamePlayer(user.Id);

            game.Players.Add(player);

            var updatedGame = await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);

            return updatedGame;
        }
    }
}