using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using NPMGame.Core.Models;

namespace NPMGame.Core.Letters
{
    public static class LettersCollection
    {
        public static Dictionary<char, Letter> Letters { get; private set; }

        public static void Init()
        {
            var lettersFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App_Data", "LettersCollection.json");
            var fileData = File.ReadAllText(lettersFile);

            var lettersCollectionData = JsonConvert.DeserializeObject<Dictionary<char, Letter>>(fileData);

            foreach (var pair in lettersCollectionData)
            {
                pair.Value.Code = pair.Key;
            }

            Letters = lettersCollectionData;
        }
    }
}