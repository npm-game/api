using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NPMGame.API.Base;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
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
    }
}