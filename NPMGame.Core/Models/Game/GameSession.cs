using System;
using System.Collections.Generic;
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

        [ForeignKey(typeof(GamePlayer))]
        public Guid CurrentTurnPlayerId { get; set; }

        public GameSession(GameOptions options)
        {
            State = GameState.NotStarted;
            Options = options;
        }
    }

    public enum GameState
    {
        NotStarted,
        Setup,
        InProgress,
        Done
    }

    public class GamePlayer
    {
        [ForeignKey(typeof(User))]
        public Guid UserId { get; set; }

        public List<Letter> Hand { get; set; }

        public int Score { get; set; }
        public double Multiplier { get; set; }
    }

    public class GameOptions
    {
        public int Goal { get; set; }
        public int HandSize { get; set; }
    }
}