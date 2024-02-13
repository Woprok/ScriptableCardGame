using System;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class ResponseMessage : CallbackMessage
    {
        public ResponseMessage(RequestMessage simpleRequest) : base()
        {
            CallbackGuid = simpleRequest?.CallbackGuid;
        }

        public bool LastCallbackInvocation { get; set; } = true;
    }
}