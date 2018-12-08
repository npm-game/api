using System.Threading.Tasks;
using Marten;
using NPMGame.Core.Base;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Repositories.Game;

namespace NPMGame.Core.Services.Game
{
    public class GameMasterService : BaseService
    {
        public GameMasterService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<GameSession> CreateGame(GameOptions options)
        {
            var game = new GameSession(options);

            var createdGame = await UnitOfWork.GetRepository<GameSessionRepository>().Create(game);

            return createdGame;
        }
    }
}