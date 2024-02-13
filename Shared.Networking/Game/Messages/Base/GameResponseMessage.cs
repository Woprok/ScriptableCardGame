using System;

namespace Shared.Networking.Game.Messages.Base
{
    [Serializable]
    public class GameResponseMessage : GameMessage
    {
        public GameResponseMessage(GameRequestMessage requestMessage) : base(requestMessage?.Request)
        {
            CallbackGuid = requestMessage?.CallbackGuid;
            Request = requestMessage?.Request;
        }

        public bool LastCallbackInvocation { get; set; } = true;
    }
}