using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Models.Game.Words;
using NSubstitute;
using NUnit.Framework;

namespace NPMGame.Core.Tests.Engine.Game
{
    [TestFixture]
    public class GameHandlerTests
    {
        private readonly ILetterGeneratorService _letterGeneratorService = Substitute.For<ILetterGeneratorService>();
        private readonly IWordMatchingService _wordMatchingService = Substitute.For<IWordMatchingService>();
        private readonly IWordScoringService _wordScoringService = Substitute.For<IWordScoringService>();

        private GameSession _game;
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

            _gameHandlerService = new GameHandlerService(_letterGeneratorService, _wordMatchingService, _wordScoringService);
        }

        [Test]
        public void TestAddPlayerToGame()
        {
            var userId = Guid.NewGuid();

            _gameHandlerService
                .UsingGame(_game)
                .AddPlayerToGame(userId);

            var game = _gameHandlerService.GetGame();

            Assert.That(game.Players, Is.TypeOf<List<GamePlayer>>());
            Assert.That(game.Players, Is.Not.Empty);
            Assert.That(game.Players, Has.One.Property("UserId").EqualTo(userId));
        }

        [Test]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(10)]
        public async Task TestStartGame(int handSize)
        {
            // Mock the letter generator to return stub Letters
            _letterGeneratorService.GenerateLetters(handSize).Returns(new Letter[handSize].Select(l => new Letter()).ToList());

            // Setup game with proper options
            _game.Options.HandSize = handSize;

            var userIds = new[]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            await _gameHandlerService
                .UsingGame(_game)
                .AddPlayerToGame(userIds[0])
                .AddPlayerToGame(userIds[1])
                .AddPlayerToGame(userIds[2])
                .StartGame();

            var game = _gameHandlerService.GetGame();

            // Check if game properties are set to proper states
            Assert.That(game.State, Is.Not.Null);
            Assert.That(game.State, Is.EqualTo(GameState.InProgress));
            Assert.That(game.StartTime, Is.Not.Null);
            Assert.That(game.StartTime, Is.TypeOf<DateTime>());

            // Check that one of the players is picked as the first to play
            Assert.That(game.CurrentTurnPlayerId, Is.Not.Null);
            Assert.That(game.CurrentTurnPlayerId, Is.AnyOf(userIds[0], userIds[1], userIds[2]));

            // Check that all players had their hands filled
            Assert.That(game.Players, Has.All.Property("Hand").Not.Null);
            Assert.That(game.Players, Has.All.Property("Hand").Not.Empty);
            Assert.That(game.Players, Has.All.Property("Hand").All.TypeOf<Letter>());
            Assert.That(game.Players, Has.All.Property("Hand").Count.EqualTo(handSize));
        }
    }
}