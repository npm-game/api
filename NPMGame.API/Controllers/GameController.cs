using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPMGame.API.Base;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Models.Exceptions;
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
            try
            {
                var game = await _gameMaster.GetGameForCurrentUser();

                return Ok(game);
            }
            catch (GameException error)
            {
                return StatusCode((int)error.ReasonCode, error.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            try
            {
                var user = await _gameMaster.GetCurrentUser();

                var game = await _gameMaster.CreateGame(user);

                return Created("", game);
            }
            catch (GameException error)
            {
                return StatusCode((int)error.ReasonCode, error.Message);
            }
        }
    }
}