using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class RequestMessage : CallbackMessage
    {
        public RequestMessage() : base() { }
    }
}