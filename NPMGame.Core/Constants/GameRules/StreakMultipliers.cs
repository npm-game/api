using System;
using System.Collections.Generic;

namespace NPMGame.Core.Constants.GameRules
{
    public static class StreakMultipliers
    {
        private static readonly Dictionary<int, double> _multipliers = new Dictionary<int, double>
        {
            {0, 1.0},
            {1, 1.0},
            {2, 1.0},
            {3, 1.1},
            {4, 1.1},
            {5, 1.2},
            {6, 1.2},
            {7, 1.4},
            {8, 1.4},
            {9, 1.5},
            {10, 2.0}
        };

        public static double GetStreakMultiplier(int streak)
        {
            streak = Math.Abs(streak);

            if (streak >= 10)
            {
                return _multipliers[10];
            }

            return _multipliers[streak];
        }
    }
}