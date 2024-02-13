using System;

namespace Shared.Networking.Common.Exceptions
{
    /// <summary>
    /// Exception to notify programmer about possible error of not subscribing to provided events
    /// </summary>
    public sealed class EventNotSubscribedException : Exception
    {
        public EventNotSubscribedException() { }

        public EventNotSubscribedException(string message) : base(message) { }

        public EventNotSubscribedException(string message, Exception inner) : base(message, inner) { }
    }
}