using System.Linq;
using NPMGame.Core.Constants;

namespace NPMGame.Core.Scoring
{
    public static class WordScoringService
    {
        public static int GetScoreForWord(string word)
        {
            var chars = word.ToCharArray();

            var letterScoreSum = chars
                .Select(c => LettersCollection.Letters[c])
                .Select(letterForChar => letterForChar.Score)
                .Sum();

            var pronounceabilityModifier = 1;

            return letterScoreSum * pronounceabilityModifier;
        }
    }
}