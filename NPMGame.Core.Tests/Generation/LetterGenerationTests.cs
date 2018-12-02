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
        public void TestLetterGenerationProbabilities()
        {

        }
    }
}