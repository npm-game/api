using System.Linq;
using NPMGame.Core.Services.Letters;

namespace NPMGame.Core.Services.Words
{
    public interface IWordScoringService
    {
        int GetScoreForWord(string word);
    }

    public class WordScoringService : IWordScoringService
    {
        public int GetScoreForWord(string word)
        {
            var chars = word.ToUpper().ToCharArray();

            var letterScoreSum = chars
                .Select(c => LettersCollection.Letters[c])
                .Select(letterForChar => letterForChar.Score)
                .Sum();

            // TODO: Do proper pronounceability scores
            var pronounceabilityModifier = 1;

            return letterScoreSum * pronounceabilityModifier;
        }
    }
}