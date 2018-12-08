using System;

namespace NPMGame.Core.Models.Exceptions
{
    public class GameException : Exception
    {
        public GameException()
        {
        }

        public GameException(string message) : base(message)
        {
        }
    }
}