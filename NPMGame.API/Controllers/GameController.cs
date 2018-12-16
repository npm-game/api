using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AlbaVulpes.API.Models.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPMGame.API.Base;
using NPMGame.API.Models.Requests;
using NPMGame.API.Repositories.Identity;
using NPMGame.API.Validators;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.API.Controllers
{
    [Route("games")]
    [Produces("application/json")]
    public class GameController : BaseController
    {
        private readonly IGameMasterService _gameMaster;

        public GameController(IGameMasterService gameMaster, IUnitOfWork unitOfWork, IValidatorService validator) : base(unitOfWork, validator)
        {
            _gameMaster = gameMaster;
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentGame()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var game = await UnitOfWork.GetRepository<GameSessionRepository>().GetGameForUser(Guid.Parse(userId));

            if (game == null)
            {
                return NotFound("You are not part of any game");
            }

            return Ok(game);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await UnitOfWork.GetRepository<UserRepository>().Get(Guid.Parse(userId));

            var game = await _gameMaster.CreateGame(user);

            return Created("", game);                                                                                                                                                                                                       
        }
    }
}