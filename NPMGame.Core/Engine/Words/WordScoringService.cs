﻿using System.Linq;
using NPMGame.Core.Engine.Letters;

namespace NPMGame.Core.Engine.Words
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