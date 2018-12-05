using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Letters;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Words;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Scoring
{
    [TestFixture]
    public class WordScoringTests
    {
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
                var matchTypeResult = WordScoringService.GetScoreForWord(pair.Key);

                Assert.That(matchTypeResult, Is.EqualTo(pair.Value));
            }
        }
    }
}