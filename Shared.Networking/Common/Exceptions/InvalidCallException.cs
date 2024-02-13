using System;

namespace Shared.Networking.Common.Exceptions
{
    /// <summary>
    /// If method was called in incorrect order, e.g. like Start before Initialize etc.
    /// </summary>
    public sealed class InvalidCallException : Exception
    {
        public InvalidCallException() { }

        public InvalidCallException(string message) : base(message) { }

        public InvalidCallException(string message, Exception inner) : base(message, inner) { }
    }
}