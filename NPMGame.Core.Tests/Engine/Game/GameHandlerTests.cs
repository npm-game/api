using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Game;
using NSubstitute;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Engine.Game
{
    [TestFixture]
    public class GameHandlerTests
    {
        private GameSession _game;

        private readonly ILetterGeneratorService _letterGeneratorService = Substitute.For<ILetterGeneratorService>();
        private readonly IWordMatchingService _wordMatchingService = Substitute.For<IWordMatchingService>();
        private readonly IWordScoringService _wordScoringService = Substitute.For<IWordScoringService>();

        private IGameHandlerService _gameHandlerService;

        [OneTimeSetUp]
        public void RunBeforeAllTests()
        {
            LettersCollection.Init();
        }

        [SetUp]
        public void RunBeforeEachTest()
        {
            _game = new GameSession();

            _gameHandlerService = new GameHandlerService(_letterGeneratorService, _wordMatchingService, _wordScoringService).UsingGame(_game);
        }

        [Test]
        public void TestAddPlayerToGame()
        {
            var userId = Guid.NewGuid();

            _gameHandlerService.AddPlayerToGame(userId);

            Assert.That(_game.Players, Is.TypeOf<List<GamePlayer>>());
            Assert.That(_game.Players, Is.Not.Empty);
            Assert.That(_game.Players, Has.One.Property("UserId").EqualTo(userId));
        }
    }
}