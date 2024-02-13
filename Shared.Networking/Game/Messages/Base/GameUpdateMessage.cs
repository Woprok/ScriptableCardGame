using System;
using Shared.Networking.Game.Enums;

namespace Shared.Networking.Game.Messages.Base
{
    [Serializable]
    public class GameUpdateMessage : GameMessage
    {
        public GameUpdateMessage(GameRequest? request) : base(request)
        {

        }
    }
}