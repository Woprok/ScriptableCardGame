using System;
using Shared.Game.Entities.Turns;
using Shared.Networking.Game.Enums;
using Shared.Networking.Game.Messages.Base;

namespace Shared.Networking.Game.Messages.Updates
{
    [Serializable]
    public class ProcessTurnUpdate : GameUpdateMessage
    {
        public ProcessTurnUpdate(Guid instance, TurnAction action, int nextPlayer) : base(GameRequest.ProcessTurn)
        {
            Instance = instance;
            Action = action;
            NextPlayer = nextPlayer;
        }

        public Guid Instance { get; set; }
        public TurnAction Action { get; set; }
        public int NextPlayer { get; set; }
    }
}