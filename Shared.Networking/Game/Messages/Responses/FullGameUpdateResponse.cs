using System;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Responses
{
    [Serializable]
    public class FullGameUpdateResponse : GameResponseMessage
    {
        public FullGameUpdateResponse(GameRequestMessage request, GameInstanceEntity instance, GameStateEntity state) : base(request)
        {
            Instance = instance;
            State = state;
        }

        public GameInstanceEntity Instance { get; set; }
        public GameStateEntity State { get; set; }
    }
}