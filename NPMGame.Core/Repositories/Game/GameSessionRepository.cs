using System;
using System.Linq;
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

        public async Task<GameSession> Get(Guid gameId)
        {
            using (var session = _store.QuerySession())
            {
                var game = await session.Query<GameSession>().Where(g => g.Id == gameId).FirstOrDefaultAsync();

                return game;
            }
        }

        public async Task<GameSession> GetGameForUser(Guid userId)
        {
            using (var session = _store.QuerySession())
            {
                var game = await session.Query<GameSession>()
                    .Where(g => g.State != GameState.Done)
                    .Where(g => g.OwnerId == userId || g.Players.Any(p => p.UserId == userId))
                    .FirstOrDefaultAsync();

                return game;
            }
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

        public async Task<GameSession> Update(GameSession game)
        {
            using (var session = _store.OpenSession())
            {
                session.Update(game);
                await session.SaveChangesAsync();

                return game;
            }
        }
    }
}