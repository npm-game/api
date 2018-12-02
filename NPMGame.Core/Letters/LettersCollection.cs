using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NPMGame.Core.Models;

namespace NPMGame.Core.Letters
{
    public static class LettersCollection
    {
        public static Dictionary<char, Letter> Letters { get; private set; }

        public static void Init()
        {
            var lettersFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LettersCollection.json");
            var fileData = File.ReadAllText(lettersFile);

            var lettersCollectionData = JsonConvert.DeserializeObject<Dictionary<char, Letter>>(fileData);

            foreach (var pair in lettersCollectionData)
            {
                pair.Value.Value = pair.Key;
            }

            Letters = lettersCollectionData;
        }
    }
}