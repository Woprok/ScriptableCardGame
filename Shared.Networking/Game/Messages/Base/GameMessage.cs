using System;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Enums;

namespace Shared.Networking.Game.Messages.Base
{
    [Serializable]
    public class GameMessage : CallbackMessage
    {
        public GameMessage(GameRequest? request) : base()
        {
            Request = request;
        }

        public GameRequest? Request { get; set; }
    }
}