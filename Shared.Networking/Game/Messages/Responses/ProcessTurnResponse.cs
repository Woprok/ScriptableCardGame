using System;
using Shared.Game.Entities.Turns;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Game.Messages.Requests;

namespace Shared.Networking.Game.Messages.Responses
{
    [Serializable]
    public class ProcessTurnResponse : GameResponseMessage
    {
        public ProcessTurnResponse(ProcessTurnRequest requestMessage, TurnActionResponse actionResponse, int nextPlayer) : base(requestMessage)
        {
            Instance = requestMessage.GameInstance;
            ActionResponse = actionResponse;
            NextPlayer = nextPlayer;
        }

        public Guid Instance { get; set; }
        public TurnActionResponse ActionResponse { get; set; }
        public int NextPlayer { get; set; }
    }
}