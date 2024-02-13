using System;
using Shared.Game.Entities.Turns;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Requests
{
    [Serializable]
    public class ProcessTurnRequest : GameRequestMessage
    {
        public ProcessTurnRequest(Guid gameInstance, TurnAction action) : base(GameRequest.ProcessTurn)
        {
            GameInstance = gameInstance;
            Action = action;
        }

        public Guid GameInstance { get; set; }
        public TurnAction Action { get; set; }
    }
}