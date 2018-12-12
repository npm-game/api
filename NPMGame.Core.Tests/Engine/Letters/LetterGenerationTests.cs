using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Models.Game.Words;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Engine.Letters
{
    [TestFixture]
    public class LetterGenerationTests
    {
        private readonly ILetterGeneratorService _letterGeneratorService;

        public LetterGenerationTests()
        {
            _letterGeneratorService = new LetterGeneratorService();
        }

        [OneTimeSetUp]
        public void RunBeforeAllTests()
        {
            LettersCollection.Init();
        }

        [Test]
        public void TestLetterGeneration()
        {
            var generated = _letterGeneratorService.GenerateLetter();

            Assert.That(generated, Is.Not.Null);
            Assert.That(generated, Is.Not.Zero);
            Assert.That(generated, Is.TypeOf<char>());
            Assert.That(generated.ToString(), Does.Match(@"^[A-Z\s]$"));
        }

        [Test]
        public async Task TestLetterGenerationProbabilities()
        {
            const int samples = 1000000;

            var generatedLetters = await _letterGeneratorService.GenerateLetters(samples);

            var generatedLetterCounts = new Dictionary<char, int>();

            foreach (var letter in generatedLetters)
            {
                if (!generatedLetterCounts.ContainsKey(letter))
                {
                    generatedLetterCounts[letter] = 0;
                }

                generatedLetterCounts[letter]++;
            }

            // Assert that all letters are generated at least once
            Assert.That(generatedLetterCounts.Keys, Is.EquivalentTo(LettersCollection.Letters.Keys));

            // Get occurrences of generated letters
            var totalPossibleOccurrences = LettersCollection.TotalPossibleOccurrences;

            var sortedExpectedOccurrenceRatios = LettersCollection.Letters
                .OrderBy(x => x.Key)
                .Select(x => x.Value.OccurrenceCount)
                .ToList();

            var sortedGeneratedOccurenceRatios = generatedLetterCounts
                .OrderBy(x => x.Key)
                .Select(x => ((double)x.Value) / samples * totalPossibleOccurrences)
                .ToList();

            // Assert that occurrence ratios are within reasonable error rate of expected occurrences
            Assert.That(sortedGeneratedOccurenceRatios, Is.EqualTo(sortedExpectedOccurrenceRatios).Within(0.05));
        }
    }
}