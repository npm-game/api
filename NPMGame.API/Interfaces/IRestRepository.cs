using System;
using System.Threading.Tasks;
using NPMGame.API.Base;

namespace NPMGame.API.Interfaces
{
    public interface IRestRepository<TModel> where TModel : ApiModel
    {
        Task<TModel> Get(Guid id);
        Task<TModel> Create(TModel data);
        Task<TModel> Update(Guid id, TModel data);
        Task<TModel> Delete(Guid id);
    }
}