using System;

namespace Spotty.Exceptions
{
    public class PlayerFailedException : Exception
    {
        public PlayerFailedException()
        {
        }

        public PlayerFailedException(string message) : base(message)
        {
        }

        public PlayerFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
