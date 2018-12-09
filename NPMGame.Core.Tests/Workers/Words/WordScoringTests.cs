using System.Collections.Generic;
using NPMGame.Core.Services.Letters;
using NPMGame.Core.Services.Words;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Workers.Words
{
    [TestFixture]
    public class WordScoringTests
    {
        private readonly IWordScoringService _wordScoringService;

        public WordScoringTests()
        {
            _wordScoringService = new WordScoringService();
        }

        [OneTimeSetUp]
        public void RunBeforeAllTests()
        {
            LettersCollection.Init();
        }

        [Test]
        public void TestWordScoring()
        {
            var wordScoringTestDefintions = new Dictionary<string, int>
            {
                {"axios", 12},
                {"crikey", 15},
                {"hello", 8},
                {"bamboozle", 24},
                {"technicallios", 20},
                {"himynameiscarl", 26},
                {"carlthatkillspeople", 32}
            };

            foreach (var pair in wordScoringTestDefintions)
            {
                var matchTypeResult = _wordScoringService.GetScoreForWord(pair.Key);

                Assert.That(matchTypeResult, Is.EqualTo(pair.Value));
            }
        }
    }
}