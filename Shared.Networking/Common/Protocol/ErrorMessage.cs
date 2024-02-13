using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class ErrorMessage : CallbackMessage
    {
        public ErrorMessage(Exception exception) : base()
        {
            Exception = exception;
        }

        public Exception Exception { get; set; }
    }
}