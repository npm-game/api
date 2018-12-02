﻿using System;
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

            return (int)((BitConverter.ToUInt32(byteArray, 0) % totalWeight) + 1);
        }
    }
}