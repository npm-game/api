using System;
using System.Collections.Generic;
using Marten.Schema;

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