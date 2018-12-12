using System;
using System.Collections.Generic;
using Marten.Schema;
using NPMGame.Core.Constants.GameRules;
using NPMGame.Core.Models.Game.Words;
using NPMGame.Core.Models.Identity;

namespace NPMGame.Core.Models.Game
{
    public class GamePlayer
    {
        [ForeignKey(typeof(User))]
        public Guid UserId { get; set; }

        public List<char> Hand { get; set; }

        public int Score { get; set; }
        public int Streak { get; set; }

        public double Multiplier => StreakMultipliers.GetStreakMultiplier(Streak);

        public GamePlayer()
        {
            Hand = new List<char>();
        }
    }
}