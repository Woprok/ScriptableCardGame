using System;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Updates
{
    [Serializable]
    public class CreateGameUpdate : GameUpdateMessage
    {
        public CreateGameUpdate(GameInstanceEntity gameInstance) : base(GameRequest.CreateGame)
        {
            GameInstance = gameInstance;
        }

        public GameInstanceEntity GameInstance { get; set; }
    }
}