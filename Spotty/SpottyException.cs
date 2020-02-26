using System;

namespace Spotty.Exceptions
{
    public class SpottyException : Exception
    {
        public SpottyException()
        {
        }

        public SpottyException(string message) : base(message)
        {
        }

        public SpottyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
