using System;
using System.Linq;
using System.Security.Cryptography;
using NPMGame.Core.Models;

namespace NPMGame.Core.Letters
{
    public static class LettersGenerator
    {
        private static readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();

        public static Letter GenerateLetter()
        {
            var lettersSortedByOccurence = LettersCollection.Letters.Values
                .OrderBy(l => l.OccurrenceCount);

            Letter selectedLetter = null;

            var rand = GetRandomNumber();

            var cumulative = 0.0;
            foreach (var letter in lettersSortedByOccurence)
            {
                cumulative += letter.Score;

                if (rand < cumulative)
                {
                    selectedLetter = letter;
                    break;
                }
            }

            return selectedLetter;
        }

        private static uint GetRandomNumber()
        {
            var byteArray = new byte[4];
            _rngProvider.GetBytes(byteArray);

            return BitConverter.ToUInt32(byteArray, 0);
        }
    }
}