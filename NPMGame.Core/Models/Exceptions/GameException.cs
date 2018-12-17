using System;
using System.Collections.Generic;
using System.Net;

namespace NPMGame.Core.Models.Exceptions
{
    public class GameException : Exception
    {
        public HttpStatusCode ReasonCode { get; }

        public GameException(string message, HttpStatusCode code = HttpStatusCode.InternalServerError) : base(message)
        {
            ReasonCode = code;
        }
    }
}