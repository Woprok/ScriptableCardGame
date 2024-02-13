using System;
using System.Collections.Generic;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Entities.Turns
{
    [Serializable]
    public class TurnAction
    {
        public TurnAction()
        {
            UsedAction = false;
        }

        public TurnAction(CardAction playedAction)
        {
            UsedAction = true;
            PlayedAction = playedAction;
        }

        public bool UsedAction { get; set; }

        public CardAction PlayedAction { get; set; }
    }

    [Serializable]
    public class CardAction
    {
        public Guid ActionId { get; set; } = Guid.NewGuid();
    }

    [Serializable]
    public class AttackAction : CardAction
    {
        public AttackAction(CardInfo sourceCard, CardInfo targetCard, HashSet<SimpleMoveResult> moveResults)
        {
            SourceCard = sourceCard;
            TargetCard = targetCard;
            MoveResults = moveResults;
        }

        public CardInfo SourceCard { get; set; }
        public CardInfo TargetCard { get; set; }
        public HashSet<SimpleMoveResult> MoveResults { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is AttackAction attack)
            {
                return SourceCard == attack.SourceCard && TargetCard == attack.TargetCard;
            }

            return false;
        }
    }

    [Serializable]
    public class MoveAction : CardAction
    {
        public MoveAction(CardInfo sourceCard, CardInfo targetPosition)
        {
            SourceCard = sourceCard;
            TargetPosition = targetPosition;
        }

        public CardInfo SourceCard { get; set; }
        public CardInfo TargetPosition { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is MoveAction move)
            {
                return SourceCard == move.SourceCard && TargetPosition == move.TargetPosition;
            }

            return false;
        }
    }

    [Serializable]
    public class CardInfo
    {
        public CardInfo(Player player, ICard card, int row, int column, bool? isNewCard)
        {
            Player = player;
            Card = card;
            Row = row;
            Column = column;
            IsNewCard = isNewCard;
        }

        public Player Player { get; set; }
        public ICard Card { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool? IsNewCard { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CardInfo info)
            {
                return Player.Account == info.Player.Account && Card.CurrentOwner == info.Card.CurrentOwner && Row == info.Row && Column == info.Column && IsNewCard == info.IsNewCard;
            }

            return false;
        }
    }
}