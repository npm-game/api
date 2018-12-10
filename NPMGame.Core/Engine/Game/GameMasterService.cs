using System;
using System.Threading.Tasks;
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
        private readonly ILetterGeneratorService _letterGeneratorService;
        private readonly IWordMatchingService _wordMatchingService;
        private readonly IWordScoringService _wordScoringService;

        public GameMasterService(ILetterGeneratorService letterGeneratorService, IWordMatchingService wordMatchingService, IWordScoringService wordScoringService, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _letterGeneratorService = letterGeneratorService;
            _wordMatchingService = wordMatchingService;
            _wordScoringService = wordScoringService;
        }

        public async Task<GameSession> CreateGame(Guid creatorUserId, GameOptions options = null)
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

            using (var handler = CreateHandler(game))
            {
                return await handler.AddPlayerToGame(creatorUserId);
            }
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
            return new GameHandlerService(game, _letterGeneratorService, _wordMatchingService, _wordScoringService, UnitOfWork);
        }
    }
}