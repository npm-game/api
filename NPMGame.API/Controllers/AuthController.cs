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
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Services;

namespace NPMGame.API.Controllers
{
    [Route("auth")]
    [Produces("application/json")]
    public class AuthController : BaseController
    {
        public AuthController(IUnitOfWork unitOfWork, IValidatorService validator) : base(unitOfWork, validator)
        {
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Status()
        {
            var user = HttpContext.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var dbUser = await UnitOfWork.GetRepository<UserRepository>().Get(Guid.Parse(userId));

            return Ok(new StatusResponse
            {
                Email = dbUser.Email,
                UserName = dbUser.UserName
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var validation = ValidatorService.GetValidator<LoginRequestValidator>().Validate(request);

            if (!validation.IsValid)
            {
                var error = validation.Errors.FirstOrDefault();
                return BadRequest(error?.ErrorMessage);
            }

            var claimsPrincipal = await UnitOfWork.GetRepository<AuthRepository>().AuthenticateUser(request);

            if (claimsPrincipal == null)
            {
                return Unauthorized();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Ok();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var validation = ValidatorService.GetValidator<RegisterRequestValidator>().Validate(request);

            if (!validation.IsValid)
            {
                var error = validation.Errors.FirstOrDefault();
                return BadRequest(error?.ErrorMessage);
            }

            var createdUser = await UnitOfWork.GetRepository<AuthRepository>().RegisterNewUser(request);

            if (createdUser == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }
    }
}