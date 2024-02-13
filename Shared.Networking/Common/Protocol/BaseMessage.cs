using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class BaseMessage : CoreMessage
    {
        public BaseMessage() : base() { }

        public bool HasError { get; set; } = false;
    }
}