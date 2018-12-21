using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Engine.Game;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Models.Game.Turn;
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
            _game = new GameSession
            {
                Options = new GameOptions
                {
                    Goal = 1000,
                    HandSize = 7
                }
            };

            _gameHandlerService = new GameHandlerService(_letterGeneratorService, _wordMatchingService, _wordScoringService);

            _letterGeneratorService.GenerateLetters(_game.Options.HandSize).Returns(new char[_game.Options.HandSize].ToList());
        }

        [TestFixture]
        public class AddPlayerToGame : GameHandlerTests
        {
            [SetUp]
            public void RunBeforeEachSubTest()
            {
                _gameHandlerService.UsingGame(_game);
            }

            [Test]
            public void ShouldAddPlayerToGame()
            {
                var userId = Guid.NewGuid();

                _gameHandlerService.AddPlayerToGame(userId);

                var game = _gameHandlerService.GetGame();

                Assert.That(game.Players, Is.TypeOf<List<GamePlayer>>());
                Assert.That(game.Players, Is.Not.Empty);
                Assert.That(game.Players, Has.One.Property("UserId").EqualTo(userId));
            }
        }

        [TestFixture]
        public class StartGame : GameHandlerTests
        {
            [SetUp]
            public void RunBeforeEachSubTest()
            {
                _gameHandlerService.UsingGame(_game);
            }

            [Test]
            public void ShouldCheckPlayersCount()
            {
                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.StartGame();
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.GameErrors.NotEnoughPlayers));
            }

            [Test]
            public async Task ShouldSetupGameInitialState()
            {
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
            }

            [Test]
            [TestCase(5)]
            [TestCase(7)]
            [TestCase(10)]
            public async Task ShouldDealLettersToAllPlayers(int handSize)
            {
                // Mock the letter generator to return stub Letters
                _letterGeneratorService.GenerateLetters(handSize).Returns(new char[handSize].ToList());

                // Setup game with proper options
                _game.Options.HandSize = handSize;

                await _gameHandlerService
                    .UsingGame(_game)
                    .AddPlayerToGame(Guid.NewGuid())
                    .AddPlayerToGame(Guid.NewGuid())
                    .AddPlayerToGame(Guid.NewGuid())
                    .StartGame();

                var game = _gameHandlerService.GetGame();

                // Check that all players had their hands filled
                Assert.That(game.Players, Has.All.Property("Hand").Not.Null);
                Assert.That(game.Players, Has.All.Property("Hand").Not.Empty);
                Assert.That(game.Players, Has.All.Property("Hand").All.TypeOf<char>());
                Assert.That(game.Players, Has.All.Property("Hand").Count.EqualTo(handSize));
            }
        }

        [TestFixture]
        public class TakeTurn : GameHandlerTests
        {
            [SetUp]
            public void RunBeforeEachSubTest()
            {
                var userIds = new[]
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                };

                _gameHandlerService
                    .UsingGame(_game)
                    .AddPlayerToGame(userIds[0])
                    .AddPlayerToGame(userIds[1])
                    .AddPlayerToGame(userIds[2]);
            }

            [Test]
            public void ShouldCheckIfGameHasStarted()
            {
                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.TakeTurn(new GameTurnAction
                    {
                        PlayerId = Guid.NewGuid()
                    });
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.GameErrors.GameNotInProgress));
            }

            [Test]
            public async Task ShouldCheckIfGameHasPlayer()
            {
                await _gameHandlerService.StartGame();

                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.TakeTurn(new GameTurnAction
                    {
                        PlayerId = Guid.NewGuid()
                    });
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.UserErrors.UserNotPlayer));
            }

            [Test]
            public async Task ShouldCheckIfItsPlayersTurn()
            {
                await _gameHandlerService.StartGame();

                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.TakeTurn(new GameTurnAction
                    {
                        PlayerId = _game.Players[1].UserId
                    });
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.NotYourTurn));
            }

            [Test]
            public async Task ShouldValidateTypeOfAction()
            {
                await _gameHandlerService.StartGame();

                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.TakeTurn(new GameTurnAction
                    {
                        PlayerId = _game.Players[0].UserId
                    });
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.InvalidTurnAction));
            }

            [Test]
            public async Task ShouldCheckIfPlayerCanGuessWord()
            {
                await _gameHandlerService.StartGame();

                var player = _game.Players[0];
                player.Hand = "txdte".ToCharArray().ToList();

                var exception = Assert.ThrowsAsync<GameException>(async () =>
                {
                    await _gameHandlerService.TakeTurn(new GameTurnGuessAction
                    {
                        PlayerId = _game.Players[0].UserId,
                        WordGuessed = "testword"
                    });
                });

                Assert.That(exception.Message, Is.EqualTo(ErrorMessages.PlayerCannotPlayWord));
            }

            [Test]
            [TestCase(0, 0)]
            [TestCase(25, 0)]
            [TestCase(25, -3)]
            public async Task ShouldProcessGuessActionPartialMatch(int playerScore, int playerStreak)
            {
                _wordMatchingService.MatchWordAgainstNPM(Arg.Any<string>()).Returns(MatchType.Partial);

                await _gameHandlerService.StartGame();

                const string wordGuessed = "testword";

                var player = _game.Players[0];

                player.Score = playerScore;
                player.Streak = playerStreak;
                player.Hand.AddRange("tsuowdryxdte".ToCharArray());

                await _gameHandlerService.TakeTurn(new GameTurnGuessAction
                {
                    PlayerId = _game.Players[0].UserId,
                    WordGuessed = wordGuessed
                });

                Assert.That(player.Score, Is.EqualTo(playerScore));

                if (playerStreak > 0)
                {
                    Assert.That(player.Streak, Is.Zero);
                }
                else
                {
                    Assert.That(player.Streak, Is.EqualTo(playerStreak));
                }
            }

            [Test]
            [TestCase(10, 0, 0, 0, -1)]
            [TestCase(10, 100, 0, 0, -1)]
            [TestCase(10, 250, 2, 150, 0)]
            [TestCase(10, 300, -3, 190, -4)]
            public async Task ShouldProcessGuessActionExactMatch(int wordScore, int playerScore, int playerStreak, int resultScore, int resultStreak)
            {
                _wordMatchingService.MatchWordAgainstNPM(Arg.Any<string>()).Returns(MatchType.Exact);
                _wordScoringService.GetScoreForWord(Arg.Any<string>()).Returns(wordScore);

                await _gameHandlerService.StartGame();

                const string wordGuessed = "testword";

                var player = _game.Players[0];

                player.Score = playerScore;
                player.Streak = playerStreak;
                player.Hand = "tsuowdryxdte".ToCharArray().ToList();

                await _gameHandlerService.TakeTurn(new GameTurnGuessAction
                {
                    PlayerId = _game.Players[0].UserId,
                    WordGuessed = wordGuessed
                });

                Assert.That(player.Score, Is.EqualTo(resultScore));
                Assert.That(player.Streak, Is.EqualTo(resultStreak));
            }

            [Test]
            [TestCase(10, 0, 0, 100, 1)]
            [TestCase(10, 100, 0, 200, 1)]
            [TestCase(10, 250, 2, 350, 3)]
            [TestCase(10, 300, -3, 410, 0)]
            public async Task ShouldProcessGuessActionNoneMatch(int wordScore, int playerScore, int playerStreak, int resultScore, int resultStreak)
            {
                _wordMatchingService.MatchWordAgainstNPM(Arg.Any<string>()).Returns(MatchType.None);
                _wordScoringService.GetScoreForWord(Arg.Any<string>()).Returns(wordScore);

                await _gameHandlerService.StartGame();

                const string wordGuessed = "testword";

                var player = _game.Players[0];

                player.Score = playerScore;
                player.Streak = playerStreak;
                player.Hand = "tsuowdryxdte".ToCharArray().ToList();

                await _gameHandlerService.TakeTurn(new GameTurnGuessAction
                {
                    PlayerId = _game.Players[0].UserId,
                    WordGuessed = wordGuessed
                });

                Assert.That(player.Score, Is.EqualTo(resultScore));
                Assert.That(player.Streak, Is.EqualTo(resultStreak));
            }

            [Test]
            [TestCase("fskhi", true)]
            [TestCase("i", true)]
            [TestCase("irweqf", false)]
            [TestCase("46832", false)]
            [TestCase("[[]qksh-=", false)]
            public async Task ShouldProcessSwitchAction(string charactersToRemove, bool success)
            {
                await _gameHandlerService.StartGame();

                var player = _game.Players[0];
                player.Hand = "ksbfgih".ToCharArray().ToList();

                var turnAction = new GameTurnSwitchAction
                {
                    PlayerId = _game.Players[0].UserId,
                    CharactersSwitched = charactersToRemove.ToCharArray().ToList()
                };

                if (success)
                {
                    // Mock the letter generator to return stub Letters
                    _letterGeneratorService.GenerateLetters(charactersToRemove.Length).Returns(new char[charactersToRemove.Length].ToList());

                    await _gameHandlerService.TakeTurn(turnAction);

                    Assert.That(player.Hand, Is.Not.Null);
                    Assert.That(player.Hand, Is.Not.Empty);
                    Assert.That(player.Hand, Is.All.TypeOf<char>());
                    Assert.That(player.Hand, Has.Count.EqualTo(_game.Options.HandSize));
                }
                else
                {
                    var exception = Assert.ThrowsAsync<GameException>(async () =>
                    {
                        await _gameHandlerService.TakeTurn(turnAction);
                    });

                    Assert.That(exception.Message, Is.EqualTo(ErrorMessages.LetterNotInPlayerHand));
                }
            }

            [Test]
            [TestCase(10, GameState.InProgress)]
            [TestCase(90, GameState.InProgress)]
            [TestCase(99, GameState.InProgress)]
            [TestCase(100, GameState.Done)]
            [TestCase(101, GameState.Done)]
            [TestCase(110, GameState.Done)]
            public async Task ShouldEndGameWhenAPlayerWins(int wordScore, GameState stateAfterTurn)
            {
                _wordMatchingService.MatchWordAgainstNPM(Arg.Any<string>()).Returns(MatchType.None);
                _wordScoringService.GetScoreForWord(Arg.Any<string>()).Returns(wordScore);

                await _gameHandlerService.StartGame();

                const string wordGuessed = "testword";

                var player = _game.Players[0];
                player.Hand = "tsuowdryxdte".ToCharArray().ToList();

                await _gameHandlerService.TakeTurn(new GameTurnGuessAction
                {
                    PlayerId = _game.Players[0].UserId,
                    WordGuessed = wordGuessed
                });

                Assert.That(_game.State, Is.EqualTo(stateAfterTurn));

                if (stateAfterTurn == GameState.Done)
                {
                    Assert.That(_game.Winner.UserId, Is.EqualTo(player.UserId));
                }
            }
        }
    }
}