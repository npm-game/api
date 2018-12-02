using System;
using System.Threading.Tasks;
using Marten;
using NPMGame.API.Interfaces;

namespace NPMGame.API.Base
{
    public class RestRepository<TModel> : ApiRepository, IRestRepository<TModel> where TModel : ApiModel
    {
        public RestRepository(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public virtual async Task<TModel> Get(Guid id)
        {
            using (var session = _store.QuerySession())
            {
                var data = await session.LoadAsync<TModel>(id);

                return data;
            }
        }

        public virtual async Task<TModel> Create(TModel data)
        {
            using (var session = _store.OpenSession())
            {
                session.Insert(data);
                await session.SaveChangesAsync();

                return data;
            }
        }

        public virtual async Task<TModel> Update(Guid id, TModel data)
        {
            using (var session = _store.QuerySession())
            {
                var dbData = await session.LoadAsync<TModel>(id);

                if (dbData == null)
                {
                    return null;
                }

                // Ensure that the ID is provided in the model, and if not, set it anyway
                data.Id = dbData.Id;
            }

            using (var session = _store.OpenSession())
            {
                session.Update(data);
                await session.SaveChangesAsync();

                return data;
            }
        }

        public virtual async Task<TModel> Delete(Guid id)
        {
            using (var session = _store.OpenSession())
            {
                var data = await session.LoadAsync<TModel>(id);

                if (data == null)
                {
                    return null;
                }

                session.Delete<TModel>(id);
                await session.SaveChangesAsync();

                return data;
            }
        }
    }
}