using System;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Updates
{
    [Serializable]
    public class JoinGameUpdate : GameUpdateMessage
    {
        public JoinGameUpdate(GameInstanceEntity instance) : base(GameRequest.JoinGame)
        {
            Instance = instance;
        }

        public GameInstanceEntity Instance { get; set; }
    }
}