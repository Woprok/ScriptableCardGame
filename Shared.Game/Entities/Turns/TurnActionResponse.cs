using System;

namespace Shared.Game.Entities.Turns
{
    [Serializable]
    public enum TurnActionResponse
    {
        ValidMove,
        InvalidMove,
        //If invalid move is impossible due to inconsistent game state
        CorruptedTurn,
        OutOfTurnOrder
    }
}