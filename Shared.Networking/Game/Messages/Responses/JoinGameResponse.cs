using System;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Responses
{
    [Serializable]
    public class JoinGameResponse : GameResponseMessage
    {
        public JoinGameResponse(GameRequestMessage requestMessage, Guid gameInstance) : base(requestMessage)
        {
            GameInstance = gameInstance;
        }

        public Guid GameInstance { get; set; }
    }
}