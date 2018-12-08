using Marten;

namespace NPMGame.Core.Base
{
    public class BaseRepository
    {
        protected readonly IDocumentStore _store;

        public BaseRepository(IDocumentStore documentStore)
        {
            _store = documentStore;
        }
    }
}