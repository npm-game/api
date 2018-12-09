using NPMGame.Core.Services;
using NPMGame.Core.Services.Data;

namespace NPMGame.Core.Base
{
    public class BaseService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}