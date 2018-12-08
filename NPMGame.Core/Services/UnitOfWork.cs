using System;
using Marten;
using NPMGame.Core.Base;

namespace NPMGame.Core.Services
{
    public interface IUnitOfWork
    {
        IDocumentStore GetStore();
        TRepository GetRepository<TRepository>() where TRepository : BaseRepository;
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDocumentStore _store;
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

        public TRepository GetRepository<TRepository>() where TRepository : BaseRepository
        {
            return (TRepository)_serviceProvider.GetService(typeof(TRepository));
        }
    }
}