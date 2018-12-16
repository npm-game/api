using Microsoft.AspNetCore.SignalR;
using NPMGame.Core.Services;

namespace NPMGame.API.Base
{
    public abstract class BaseHub : Hub
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IValidatorService ValidatorService;

        protected BaseHub(IUnitOfWork unitOfWork, IValidatorService validator)
        {
            UnitOfWork = unitOfWork;
            ValidatorService = validator;
        }
    }
}