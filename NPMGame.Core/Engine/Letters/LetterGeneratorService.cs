using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NPMGame.Core.Models.Game.Words;

namespace NPMGame.Core.Engine.Letters
{
    public interface ILetterGeneratorService
    {
        Task<List<char>> GenerateLetters(int count);
        char GenerateLetter();
    }

    public class LetterGenerationException : Exception
    {
        public LetterGenerationException(string message) : base(message)
        {
        }
    }

    public class LetterGeneratorService : ILetterGeneratorService
    {
        private static readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();

        public async Task<List<char>> GenerateLetters(int count)
        {
            var letterTasks = new List<Task<Letter>>(new Task<Letter>[count])
                .Select(x => Task.Run(() => GenerateLetter()));

            return (await Task.WhenAll(letterTasks)).ToList();
        }

        public char GenerateLetter()
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

            if (selectedLetter == null)
            {
                throw new LetterGenerationException("Letter could not be generated");
            }

            return selectedLetter.Code;
        }

        private static int GetRandomWeight(int totalWeight)
        {
            var byteArray = new byte[4];
            _rngProvider.GetBytes(byteArray);

            return (int)(BitConverter.ToUInt32(byteArray, 0) % totalWeight);
        }
    }
}