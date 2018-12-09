using System;
using System.Collections.Generic;

namespace NPMGame.Core.Models.Game.Turn
{
    public class GameTurnAction
    {
        public Guid PlayerId { get; set; }
    }

    public class GameTurnGuessAction : GameTurnAction
    {
        public string WordGuessed { get; set; }
    }

    public class GameTurnSwitchAction : GameTurnAction
    {
        public List<char> CharactersSwitched { get; set; }
    }
}