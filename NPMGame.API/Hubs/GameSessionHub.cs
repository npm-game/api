using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NPMGame.API.Base;
using NPMGame.Core.Engine.Game;
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

        public async Task StartGame()
        {
            var game = await _gameMaster.GetGameForCurrentUser();

            await _gameMaster.TryAuthorizeUserAsOwner(game);

            var handler = _gameMaster.CreateHandler(game);

            await handler.StartGame();

            game = handler.GetGame();

            await Clients.Group(game.Id.ToString()).SendAsync("GameStarted", game);
        }
    }
}