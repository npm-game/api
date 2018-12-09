using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Baseline;
using NPMGame.Core.Models.Game.Words;

namespace NPMGame.Core.Workers.Letters
{
    public static class LettersGenerator
    {
        private static readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();

        public static async Task<List<Letter>> GenerateLetters(int count)
        {
            var letterTasks = new List<Task<Letter>>(new Task<Letter>[count])
                .Select(x => Task.Run(() => GenerateLetter()));

            return (await Task.WhenAll(letterTasks)).ToList();
        }

        public static Letter GenerateLetter()
        {
            var lettersSortedByOccurence = LettersCollection.Letters.Values
                .OrderBy(l => l.OccurrenceCount)
                .ToList();

            Letter selectedLetter = null;

            var rand = GetRandomWeight(lettersSortedByOccurence.Select(l => l.OccurrenceCount).Sum());

            var cumulative = 0.0;
            foreach (var letter in lettersSortedByOccurence)
            {
                cumulative += letter.OccurrenceCount;

                if (rand < cumulative)
                {
                    selectedLetter = letter;
                    break;
                }
            }

            return selectedLetter;
        }

        private static int GetRandomWeight(int totalWeight)
        {
            var byteArray = new byte[4];
            _rngProvider.GetBytes(byteArray);

            return (int)(BitConverter.ToUInt32(byteArray, 0) % totalWeight);
        }
    }
}