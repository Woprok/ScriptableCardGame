using System;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Updates
{
    [Serializable]
    public class FullGameUpdate : GameUpdateMessage
    {
        public FullGameUpdate(GameInstanceEntity instance, GameStateEntity state) : base(GameRequest.ObtainFullState)
        {
            Instance = instance;
            State = state;
        }

        public GameInstanceEntity Instance { get; set; }
        public GameStateEntity State { get; set; }
    }
}