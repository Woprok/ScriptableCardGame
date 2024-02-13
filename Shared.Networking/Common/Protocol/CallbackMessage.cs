using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class CallbackMessage : BaseMessage
    {
        public CallbackMessage() : base() { }

        public Guid? CallbackGuid { get; set; } = null;
    }
}