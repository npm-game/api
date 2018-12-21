using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NPMGame.API.Base;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game.Turn;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Services;

namespace NPMGame.API.Hubs
{
    public class GameSessionHub : BaseHub
    {
        private readonly IGameMasterService _gameMaster;

        public GameSessionHub(IGameMasterService gameMaster, IUnitOfWork unitOfWork, IValidatorService validator) : base(unitOfWork, validator)
        {
            _gameMaster = gameMaster;
        }

        [Authorize]
        public async Task JoinGame(string inviteToken)
        {
            // TODO: Use proper encrypted token to determine game id
            var gameId = Guid.Parse(inviteToken);

            var user = await _gameMaster.GetCurrentUser();
            var game = await UnitOfWork.GetRepository<GameSessionRepository>().Get(gameId);

            var handler = _gameMaster.CreateHandler(game)
                .AddPlayerToGame(user.Id);

            game = handler.GetGame();

            await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

            await Clients.Group(game.Id.ToString()).SendAsync("game:update", game);
            await Clients.Group(gameId.ToString()).SendAsync("game:log", DateTime.Now.ToString("s") + " - " + user.UserName + " has joined the game.");
        }

        [Authorize]
        public async Task StartGame()
        {
            var game = await _gameMaster.GetGameForCurrentUser();

            try
            {
                await _gameMaster.TryAuthorizeUserAsOwner(game);

                await Clients.Group(game.Id.ToString()).SendAsync("game:log", DateTime.Now.ToString("s") + " - " + Context.User.Identity.Name + " was authorized as the owner of the game.");

                var handler = _gameMaster.CreateHandler(game);

                await handler.StartGame();

                game = handler.GetGame();

                await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);

                await Clients.Group(game.Id.ToString()).SendAsync("game:update", game);
            }
            catch (GameException error)
            {
                await Clients.Group(game.Id.ToString()).SendAsync("game:log", DateTime.Now.ToString("s") + " - Error: " + error.Message);
            }
        }

        [Authorize]
        public async Task TakeTurn(GameTurnGuessAction turnAction)
        {
            var user = await _gameMaster.GetCurrentUser();
            var game = await _gameMaster.GetGameForCurrentUser(user);

            turnAction.PlayerId = user.Id;

            try
            {
                await _gameMaster.TryAuthorizeUserAsPlayer(game);

                await Clients.Group(game.Id.ToString()).SendAsync("game:log", DateTime.Now.ToString("s") + " - " + Context.User.Identity.Name + " was authorized as a player of the game.");

                var handler = _gameMaster.CreateHandler(game);

                await handler.TakeTurn(turnAction);

                game = handler.GetGame();

                await UnitOfWork.GetRepository<GameSessionRepository>().Update(game);

                await Clients.Group(game.Id.ToString()).SendAsync("game:update", game);
            }
            catch (GameException error)
            {
                await Clients.Group(game.Id.ToString()).SendAsync("game:log", DateTime.Now.ToString("s") + " - Error: " + error.Message);
            }
        }

        // TODO Need to remove characters from player's hand after word is played
        // TODO Need to replenish player hand after
        // TODO Need to switch user to next player


        // TODO
        //public async Task LeaveGame()
        //{
        //}
    }
}