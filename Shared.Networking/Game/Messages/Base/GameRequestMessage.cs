using System;
using Shared.Networking.Game.Enums;

namespace Shared.Networking.Game.Messages.Base
{
    [Serializable]
    public class GameRequestMessage : GameMessage
    {
        public GameRequestMessage(GameRequest? request) : base(request) { }
    }
}