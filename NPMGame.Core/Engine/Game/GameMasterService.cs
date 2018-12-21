using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        IGameHandlerService CreateHandler(GameSession game);

        Task<User> GetCurrentUser();
        Task<GameSession> GetGameForCurrentUser(User user = null);
        Task TryAuthorizeUserAsOwner(GameSession game = null, User user = null);
        Task TryAuthorizeUserAsPlayer(GameSession game = null, User user = null);
    }

    public class GameMasterService : BaseService, IGameMasterService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameMasterService(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
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
                Options = options,
                OwnerId = creatorUser.Id
            };

            game = await UnitOfWork.GetRepository<GameSessionRepository>().Create(game);

            return game;
        }

        public async Task<User> GetCurrentUser()
        {
            var context = _httpContextAccessor.HttpContext;
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new GameException(ErrorMessages.UserErrors.UserNotAuthorized, HttpStatusCode.Unauthorized);
            }

            var currentUser = await UnitOfWork.GetRepository<UserRepository>().Get(Guid.Parse(userId));

            if (currentUser == null)
            {
                throw new GameException(ErrorMessages.UserNotFound, HttpStatusCode.NotFound);
            }

            return currentUser;
        }

        public async Task<GameSession> GetGameForCurrentUser(User user = null)
        {
            if (user == null)
            {
                user = await GetCurrentUser();
            }

            var game = await UnitOfWork.GetRepository<GameSessionRepository>().GetGameForUser(user.Id);

            if (game == null)
            {
                throw new GameException(ErrorMessages.UserErrors.UserNotInAnyGame, HttpStatusCode.NotFound);
            }

            return game;
        }

        public async Task TryAuthorizeUserAsOwner(GameSession game = null, User user = null)
        {
            if (user == null)
            {
                user = await GetCurrentUser();
            }
            if (game == null)
            {
                game = await GetGameForCurrentUser(user);
            }

            await TryAuthorizeUserAsPlayer(game, user);

            if (user.Id != game.OwnerId)
            {
                throw new GameException(ErrorMessages.UserErrors.UserNotOwner, HttpStatusCode.Forbidden);
            }
        }

        public async Task TryAuthorizeUserAsPlayer(GameSession game = null, User user = null)
        {
            if (user == null)
            {
                user = await GetCurrentUser();
            }
            if (game == null)
            {
                game = await GetGameForCurrentUser(user);
            }

            if (!game.Players.Any(p => p.UserId == user.Id))
            {
                throw new GameException(ErrorMessages.UserErrors.UserNotPlayer);
            }
        }

        public IGameHandlerService CreateHandler(GameSession game)
        {
            var handler = _serviceProvider.GetRequiredService<IGameHandlerService>();

            return handler.UsingGame(game);
        }
    }
}