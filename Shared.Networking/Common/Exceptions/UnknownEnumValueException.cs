using System;

namespace Shared.Networking.Common.Exceptions
{
    public sealed class UnknownEnumValueException : Exception
    {
        public UnknownEnumValueException() { }

        public UnknownEnumValueException(string message) : base(message) { }

        public UnknownEnumValueException(string message, Exception inner) : base(message, inner) { }
    }
}