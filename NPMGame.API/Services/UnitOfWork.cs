using System;
using Marten;
using NPMGame.API.Base;

namespace NPMGame.API.Services
{
    public interface IUnitOfWork
    {
        IDocumentStore GetStore();
        TRepository GetRepository<TRepository>() where TRepository : ApiRepository;
    }

    public class UnitOfWork : IUnitOfWork
    {
        protected readonly IDocumentStore _store;

        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(IDocumentStore documentStore, IServiceProvider serviceProvider)
        {
            _store = documentStore;
            _serviceProvider = serviceProvider;
        }

        public IDocumentStore GetStore()
        {
            return _store;
        }

        public TRepository GetRepository<TRepository>() where TRepository : ApiRepository
        {
            return (TRepository)_serviceProvider.GetService(typeof(TRepository));
        }
    }
}