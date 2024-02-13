using System;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Responses
{
    [Serializable]
    public class CreateGameResponse : GameResponseMessage
    {
        public CreateGameResponse(GameRequestMessage requestMessage, bool wasCreated) : base(requestMessage)
        {
            WasCreated = wasCreated;
        }

        public bool WasCreated { get; set; }
    }
}