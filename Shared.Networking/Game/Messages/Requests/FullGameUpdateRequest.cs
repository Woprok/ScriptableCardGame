using System;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Requests
{
    [Serializable]
    public class FullGameUpdateRequest : GameRequestMessage
    {
        public FullGameUpdateRequest(Guid gameInstance) : base(GameRequest.ObtainFullState)
        {
            GameInstance = gameInstance;
        }

        public Guid GameInstance { get; set; }
    }
}