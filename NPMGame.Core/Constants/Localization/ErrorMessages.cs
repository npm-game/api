namespace NPMGame.Core.Constants.Localization
{
    public static class ErrorMessages
    {
        // Data Exceptions
        public const string GameNotFound = "Game does not exist";
        public const string UserNotFound = "User does not exist";

        // Player Exceptions
        public const string NotYourTurn = "Can't take action. It is not this player's turn yet";
        public const string PlayerNotInGame = "Player is not a part of this game";
        public const string PlayerCannotPlayWord = "Player does not have the correct letters to play chosen word";

        // Game Exceptions
        public const string GameNotInProgress = "Game is not in progress.";
        public const string NotEnoughPlayers = "Cannot start game without at least 2 players";
    }
}