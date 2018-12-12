using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Engine.Letters;
using NPMGame.Core.Engine.Words;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Models.Game.Turn;
using NPMGame.Core.Models.Identity;

namespace NPMGame.Core.Engine.Game
{
    public interface IGameHandlerService
    {
        IGameHandlerService UsingGame(GameSession game);
        GameSession GetGame();

        IGameHandlerService AddPlayerToGame(Guid userId);
        Task<IGameHandlerService> StartGame();
        Task<IGameHandlerService> TakeTurn(GameTurnAction turnAction);
    }

    public class GameHandlerService : IGameHandlerService
    {
        private readonly ILetterGeneratorService _letterGeneratorService;
        private readonly IWordMatchingService _wordMatchingService;
        private readonly IWordScoringService _wordScoringService;

        private GameSession _game;

        public GameHandlerService(ILetterGeneratorService letterGeneratorService, IWordMatchingService wordMatchingService, IWordScoringService wordScoringService)
        {
            _letterGeneratorService = letterGeneratorService;
            _wordMatchingService = wordMatchingService;
            _wordScoringService = wordScoringService;
        }

        public IGameHandlerService UsingGame(GameSession game)
        {
            _game = game;

            return this;
        }

        public GameSession GetGame()
        {
            return _game;
        }

        public IGameHandlerService AddPlayerToGame(Guid userId)
        {
            // TODO: Create GamePlayer factory to handle user ties and all that
            var player = new GamePlayer
            {
                UserId = userId
            };

            _game.Players.Add(player);

            return this;
        }

        public async Task<IGameHandlerService> StartGame()
        {
            if (_game.Players.Count <= 1)
            {
                throw new GameException(ErrorMessages.NotEnoughPlayers);
            }

            _game.State = GameState.InProgress;
            _game.StartTime = DateTime.Now;

            // TODO: Do a dice roll to pick the first player
            _game.CurrentTurnPlayerId = _game.Players.First().UserId;

            // Fill each player's hand
            foreach (var player in _game.Players)
            {
                await FillPlayerHand(player);
            }

            return this;
        }

        public async Task<IGameHandlerService> TakeTurn(GameTurnAction turnAction)
        {
            if (_game.State != GameState.InProgress)
            {
                throw new GameException(ErrorMessages.GameNotInProgress);
            }

            var currentPlayer = _game.Players.FirstOrDefault(p => p.UserId == turnAction.PlayerId);

            if (currentPlayer == null)
            {
                throw new GameException(ErrorMessages.PlayerNotInGame);
            }

            if (_game.CurrentTurnPlayerId != turnAction.PlayerId)
            {
                throw new GameException(ErrorMessages.NotYourTurn);
            }

            // Take the turn actions that the player selected
            if (turnAction is GameTurnGuessAction guessAction)
            {
                await ProcessGuessTurn(currentPlayer, guessAction);
            }
            else if (turnAction is GameTurnSwitchAction switchAction)
            {
                await ProcessSwitchTurn(currentPlayer, switchAction);
            }
            else
            {
                throw new GameException(ErrorMessages.InvalidTurnAction);
            }

            EndPlayerTurn(currentPlayer);

            return this;
        }

        private async Task ProcessGuessTurn(GamePlayer currentPlayer, GameTurnGuessAction turnAction)
        {
            if (!CanPlayerPlayWord(currentPlayer, turnAction.WordGuessed))
            {
                throw new GameException(ErrorMessages.PlayerCannotPlayWord);
            }

            var matchType = await _wordMatchingService.MatchWordAgainstNPM(turnAction.WordGuessed);

            if (matchType == MatchType.Partial)
            {
                if (currentPlayer.Streak > 0)
                {
                    currentPlayer.Streak = 0;
                }
            }
            else
            {
                var wordScore = _wordScoringService.GetScoreForWord(turnAction.WordGuessed);

                wordScore = (int)(wordScore * currentPlayer.Multiplier * 10);

                if (matchType == MatchType.Exact)
                {
                    if (currentPlayer.Streak > 0)
                    {
                        currentPlayer.Streak = 0;
                    }
                    else
                    {
                        currentPlayer.Streak--;
                    }

                    currentPlayer.Score -= Math.Min(currentPlayer.Score, wordScore);
                }
                else if (matchType == MatchType.None)
                {
                    if (currentPlayer.Streak < 0)
                    {
                        currentPlayer.Streak = 0;
                    }
                    else
                    {
                        currentPlayer.Streak++;
                    }

                    currentPlayer.Score += wordScore;
                }
            }
        }

        private async Task ProcessSwitchTurn(GamePlayer currentPlayer, GameTurnSwitchAction turnAction)
        {
            foreach (var character in turnAction.CharactersSwitched)
            {
                var removeIndex = currentPlayer.Hand.FindIndex(l => l == character);

                if (removeIndex < 0)
                {
                    throw new GameException(ErrorMessages.LetterNotInPlayerHand);
                }

                currentPlayer.Hand.RemoveAt(removeIndex);
            }

            await FillPlayerHand(currentPlayer);
        }

        private GameSession EndPlayerTurn(GamePlayer currentPlayer)
        {
            if (currentPlayer.Score >= _game.Options.Goal)
            {
                // TODO: Do game end logic
                _game.State = GameState.Done;

                return _game;
            }

            // Move to next player
            var nextPlayerIndex = _game.Players.IndexOf(currentPlayer) + 1;
            if (nextPlayerIndex >= _game.Players.Count - 1)
            {
                nextPlayerIndex = 0;
            }

            var nextPlayer = _game.Players[nextPlayerIndex];

            _game.CurrentTurnPlayerId = nextPlayer.UserId;

            return _game;
        }

        private async Task FillPlayerHand(GamePlayer player)
        {
            var lettersInHandCount = player.Hand.Count;
            var lettersToFill = _game.Options.HandSize - lettersInHandCount;

            var generatedLetters = await _letterGeneratorService.GenerateLetters(lettersToFill);

            player.Hand.AddRange(generatedLetters);
        }

        private bool CanPlayerPlayWord(GamePlayer player, string word)
        {
            var wordCharacters = word.ToCharArray();
            var handCharacters = new List<char>(player.Hand);

            foreach (var c in wordCharacters)
            {
                if (!handCharacters.Contains(c))
                {
                    return false;
                }

                handCharacters.Remove(c);
            }

            return true;
        }
    }
}