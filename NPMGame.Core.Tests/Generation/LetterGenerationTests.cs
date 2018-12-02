using System.Collections.Generic;
using System.Threading.Tasks;
using NPMGame.Core.Letters;
using NPMGame.Core.Models;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Generation
{
    [TestFixture]
    public class LetterGenerationTests
    {
        [OneTimeSetUp]
        public void RunBeforeAllTests()
        {
            LettersCollection.Init();
        }

        [Test]
        public void TestLetterGeneration()
        {
            var generated = LettersGenerator.GenerateLetter();

            Assert.That(generated, Is.Not.Null);
            Assert.That(generated, Is.TypeOf<Letter>());

            Assert.That(generated.Code, Is.Not.Zero);
            Assert.That(generated.Score, Is.Not.Zero);
            Assert.That(generated.OccurrenceCount, Is.Not.Zero);
        }

        [Test]
        public async Task TestLetterGenerationProbabilities()
        {
            const int samples = 1000000;

            var generatedLetterTasks = new List<Task<Letter>>();

            for (var i = 0; i < samples; i++)
            {
                generatedLetterTasks.Add(Task.Run(() => LettersGenerator.GenerateLetter()));
            }

            var generatedLetters = await Task.WhenAll(generatedLetterTasks);

            var generatedLetterCounts = new Dictionary<char, int>();

            foreach (var letter in generatedLetters)
            {
                var code = letter.Code;

                if (!generatedLetterCounts.ContainsKey(code))
                {
                    generatedLetterCounts[code] = 0;
                }

                generatedLetterCounts[code]++;
            }

            // Assert that all letters are generated at least once
            Assert.That(generatedLetterCounts.Keys, Is.EquivalentTo(LettersCollection.Letters.Keys));
        }
    }
}