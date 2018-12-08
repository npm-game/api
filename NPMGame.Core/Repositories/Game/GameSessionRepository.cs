using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Marten;
using NPMGame.Core.Base;
using NPMGame.Core.Models.Game;

namespace NPMGame.Core.Repositories.Game
{
    public class GameSessionRepository : BaseRepository
    {
        public GameSessionRepository(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<GameSession> Create(GameSession game)
        {
            using (var session = _store.OpenSession())
            {
                session.Insert(game);
                await session.SaveChangesAsync();

                return game;
            }
        }
    }
}