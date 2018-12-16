using Marten;

namespace NPMGame.Core.Base
{
    public abstract class BaseRepository
    {
        protected readonly IDocumentStore _store;

        protected BaseRepository(IDocumentStore documentStore)
        {
            _store = documentStore;
        }
    }
}