using Microsoft.AspNetCore.Mvc;
using NPMGame.Core.Services;

namespace NPMGame.API.Base
{
    public class BaseController : Controller
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IValidatorService ValidatorService;

        protected BaseController(IUnitOfWork unitOfWork, IValidatorService validator)
        {
            UnitOfWork = unitOfWork;
            ValidatorService = validator;
        }
    }
}