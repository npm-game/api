using Marten;

namespace NPMGame.API.Base
{
    public class ApiRepository
    {
        protected readonly IDocumentStore _store;

        public ApiRepository(IDocumentStore documentStore)
        {
            _store = documentStore;
        }
    }
}