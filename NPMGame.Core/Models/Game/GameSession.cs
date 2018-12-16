using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Schema;
using NPMGame.Core.Models.Identity;

namespace NPMGame.Core.Models.Game
{
    public class GameSession
    {
        public Guid Id { get; set; }

        public GameState State { get; set; }
        public GameOptions Options { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<GamePlayer> Players { get; set; }

        [ForeignKey(typeof(User))]
        public Guid CurrentTurnPlayerId { get; set; }

        public GamePlayer Winner => Players.FirstOrDefault(p => p.Score >= Options.Goal);

        public GameSession()
        {
            State = GameState.NotStarted;

            Options = new GameOptions();
            Players = new List<GamePlayer>();
        }
    }

    public enum GameState
    {
        NotStarted,
        InProgress,
        Done
    }
}