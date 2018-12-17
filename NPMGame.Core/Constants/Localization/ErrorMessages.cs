namespace NPMGame.Core.Constants.Localization
{
    public static class ErrorMessages
    {
        public static class UserErrors
        {
            public const string UserNotAuthorized = "You are not authorized";
            public const string UserNotOwner = "You are not the creator of this game";
            public const string UserNotPlayer = "You are not a player in this game";
            public const string UserNotInAnyGame = "You are not a part of any game";
        }

        // Data Exceptions
        public const string GameNotFound = "Game does not exist";
        public const string UserNotFound = "User does not exist";
        
        // Player Exceptions
        public const string NotYourTurn = "Can't take action. It is not this player's turn yet";
        public const string PlayerCannotPlayWord = "Player does not have the correct letters to play chosen word";
        public const string InvalidTurnAction = "Turn action taken is invalid";
        public const string LetterNotInPlayerHand = "Letter is not in player's hand";

        // Game Exceptions
        public const string GameNotInProgress = "Game is not in progress.";
        public const string NotEnoughPlayers = "Cannot start game without at least 2 players";
    }
}