using System;
using System.Linq;
using System.Threading.Tasks;
using NPMGame.Core.Base;
using NPMGame.Core.Constants.Localization;
using NPMGame.Core.Models.Enums;
using NPMGame.Core.Models.Exceptions;
using NPMGame.Core.Models.Game;
using NPMGame.Core.Repositories.Game;
using NPMGame.Core.Repositories.Identity;
using NPMGame.Core.Workers.Letters;
using NPMGame.Core.Workers.Words;

namespace NPMGame.Core.Services.Game
{
    public class GameHandlerService : BaseService
    {
        public GameSession Game { get; private set; }

        public GameHandlerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public GameHandlerService UsingGame(GameSession game)
        {
            Game = game;

            return this;
        }

        public async Task<GameSession> AddPlayerToGame(Guid userId)
        {
            // TODO: Check if player already part of any other game
            var user = await UnitOfWork.GetRepository<UserRepository>().Get(userId);

            if (user == null)
            {
                throw new GameException(ErrorMessages.UserNotFound);
            }

            // TODO: Create GamePlayer factory to handle user ties and all that
            var player = new GamePlayer(user.Id);

            Game.Players.Add(player);

            return await SaveGame();
        }

        public async Task<GameSession> StartGame()
        {
            Game.State = GameState.InProgress;
            Game.StartTime = DateTime.Now;

            // TODO: Do a dice roll to pick the first player
            Game.CurrentTurnPlayerId = Game.Players.First().UserId;

            // Fill each player's hand
            foreach (var player in Game.Players)
            {
                await FillPlayerHand(player);
            }

            return await SaveGame();
        }

        public async Task<GameSession> TakeTurn(GameTurnAction turnAction)
        {
            var currentPlayer = Game.Players.FirstOrDefault(p => p.UserId == turnAction.PlayerId);

            if (currentPlayer == null)
            {
                throw new GameException(ErrorMessages.PlayerNotInGame);
            }

            if (Game.CurrentTurnPlayerId != turnAction.PlayerId)
            {
                throw new GameException(ErrorMessages.NotYourTurn);
            }

            if (turnAction is GameTurnGuessAction guessAction)
            {
                await ProcessGuessTurn(currentPlayer, guessAction);
            }
            if (turnAction is GameTurnSwitchAction switchAction)
            {
                await ProcessSwitchTurn(currentPlayer, switchAction);
            }

            return await SaveGame();
        }

        private async Task<GameSession> SaveGame()
        {
            return await UnitOfWork.GetRepository<GameSessionRepository>().Update(Game);
        }

        private async Task ProcessGuessTurn(GamePlayer currentPlayer, GameTurnGuessAction turnAction)
        {
            var matchType = await WordMatcher.MatchWordAgainstNPM(turnAction.WordGuessed);

            if (matchType == MatchType.Partial)
            {
                if (currentPlayer.Streak > 0)
                {
                    currentPlayer.Streak = 0;
                }
            }
            else
            {
                var wordScore = WordScorer.GetScoreForWord(turnAction.WordGuessed);

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
                    currentPlayer.Streak++;
                    currentPlayer.Score += wordScore;
                }
            }
        }

        private async Task ProcessSwitchTurn(GamePlayer currentPlayer, GameTurnSwitchAction turnAction)
        {
            foreach (var character in turnAction.CharactersSwitched)
            {
                var letterToRemove = currentPlayer.Hand.First(l => l.Code == character);
                currentPlayer.Hand.Remove(letterToRemove);
            }

            await FillPlayerHand(currentPlayer);
        }

        private async Task FillPlayerHand(GamePlayer player)
        {
            var lettersInHandCount = player.Hand.Count;
            var lettersToFill = Game.Options.HandSize - lettersInHandCount;

            var generatedLetters = await LettersGenerator.GenerateLetters(lettersToFill);

            player.Hand.AddRange(generatedLetters);
        }
    }
}