using System;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Updates
{
    [Serializable]
    public class LeaveGameUpdate : GameUpdateMessage
    {
        public LeaveGameUpdate(GameInstanceEntity instance) : base(GameRequest.CloseGame)
        {
            Instance = instance;
        }

        public GameInstanceEntity Instance { get; set; }
    }
}