using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using NPMGame.Core.Base;
using NPMGame.Core.Models.Identity;

namespace NPMGame.Core.Repositories.Identity
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<User> Get(Guid userId)
        {
            using (var session = _store.QuerySession())
            {
                var user = await session.Query<User>().Where(u => u.Id == userId).FirstOrDefaultAsync();

                return user;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            using (var session = _store.QuerySession())
            {
                var user = await session.Query<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

                return user;
            }
        }
    }
}