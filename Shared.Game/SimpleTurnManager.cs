using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Shared.Game.Entities;
using Shared.Game.Entities.Cards.CardClasses;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Entities.Cards.Stats;
using Shared.Game.Entities.GameBoards;
using Shared.Game.Entities.Turns;
using Shared.Game.Helpers;
using Shared.Game.Views;

namespace Shared.Game
{
    [Serializable]
    public enum SimpleMoveResult
    {
        SourceSurvived,
        TargetSurvived
    }

    public class VirtualCardHolder
    {
        public VirtualCardHolder(Player owner, ICard currentCard, int row, int column, bool isAvatar = false)
        {
            Owner = owner;
            CurrentCard = currentCard;
            Row = row;
            Column = column;
            IsAvatar = isAvatar;
        }

        public Player Owner { get; set; }
        public bool IsAvatar { get; set; } 
        public ICard CurrentCard { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class VirtualServerGameBoardManager
    {
        public VirtualServerGameBoardManager() { }

        //these two are split in server version fully
        private Dictionary<Player,List<VirtualCardHolder>> CardHolders { get; set; } = new Dictionary<Player, List<VirtualCardHolder>>();
        private List<VirtualCardHolder> AvatarHolders { get; set; } = new List<VirtualCardHolder>(); 

        public void CreateDefaultBoard(IList<Player> players, GameBoardSettings settings)
        {
            foreach (Player player in players)
            {
                var avatar = new VirtualCardHolder(player, player.Avatar.AvatarCard, settings.RowCount / 2, 0, true);
                AvatarHolders.Add(avatar);

                var playerHolders = new List<VirtualCardHolder>();
                for (int i = 0; i < settings.RowCount; i++)
                {
                    for (int j = 1; j < settings.ColumnCount + 1; j++)
                    {
                        var holder = new VirtualCardHolder(player, null, i, j, false);
                        playerHolders.Add(holder);
                    }
                }

                CardHolders.Add(player, playerHolders);
            }
        }

        public VirtualCardHolder FindCardHolder(CardInfo info)
        {
            if (CardHolders.TryGetValue(info.Player, out var hisCardHolders))
            {
                var holderFromInfo = hisCardHolders.FirstOrDefault(item => item.Row == info.Row && item.Column == info.Column);
                var holderFromAvatar = AvatarHolders.FirstOrDefault(item => Equals(item.Owner, info.Player) && item.Row == info.Row && item.Column == info.Column);
                if (holderFromInfo != null)
                {
                    if (holderFromInfo.CurrentCard == null)
                    {
                        holderFromInfo.CurrentCard = info.Card;
                    }
                    return holderFromInfo;
                }

                if (holderFromAvatar != null)
                {
                    if (holderFromAvatar.CurrentCard == null)
                    {
                        holderFromAvatar.CurrentCard = info.Card;
                    }
                    return holderFromAvatar;
                }
            }

            return null;
        }
        
        public HashSet<ServerTurnActionError> ProcessAction(CardAction action, out HashSet<ServerAttackResult> results, out Player sourcePlayer, out Player targetPlayer)
        {
            results = null;
            sourcePlayer = null;
            targetPlayer = null;
            HashSet<ServerTurnActionError> errors = new HashSet<ServerTurnActionError>();
            if (action is MoveAction move)
            {
                var source = FindCardHolder(move.SourceCard);
                var target = FindCardHolder(move.TargetPosition);

                if (move.SourceCard.IsNewCard == false)
                {
                    source = new VirtualCardHolder(move.SourceCard.Player, move.SourceCard.Card, move.SourceCard.Row, move.SourceCard.Column, false);
                }

                if (source == null || target == null)
                {
                    errors.Add(ServerTurnActionError.DoesNotExist);
                }
                else if (target.CurrentCard != null)
                {
                    errors.Add(ServerTurnActionError.MoveDeniedOccupied);
                }
                else if (source.CurrentCard == null)
                {
                    errors.Add(ServerTurnActionError.CardMissing);
                }
                else if (!source.CurrentCard.CanBeMoved)
                {
                    errors.Add(ServerTurnActionError.UnmoveableCard);
                }
                else
                {
                    //Finally process move
                    source.CurrentCard = null;
                    target.CurrentCard = move.SourceCard.Card;
                }
            }

            if (action is AttackAction attack)
            {
                var source = FindCardHolder(attack.SourceCard);
                var target = FindCardHolder(attack.TargetCard);

                if (attack.SourceCard.IsNewCard == false)
                {
                    source = new VirtualCardHolder(attack.SourceCard.Player, attack.SourceCard.Card, attack.SourceCard.Row, attack.SourceCard.Column, false);
                }

                if (source == null || target == null)
                {
                    errors.Add(ServerTurnActionError.DoesNotExist);
                }
                else if (source.CurrentCard == null || target.CurrentCard == null)
                {
                    errors.Add(ServerTurnActionError.CardMissing);
                }
                else if (!source.CurrentCard.CanBeMoved)
                {
                    errors.Add(ServerTurnActionError.UnmoveableCard);
                }
                else
                {
                    //Finally process attack
                    results = GetAttackResults(source, target);
                }

                sourcePlayer = attack.SourceCard.Player;
                targetPlayer = attack.TargetCard.Player;
            }

            return errors;
        }

        public HashSet<ServerAttackResult> GetAttackResults(VirtualCardHolder source, VirtualCardHolder target)
        {
            HashSet<ServerAttackResult> result = new HashSet<ServerAttackResult>();
            if (source.CurrentCard.Class is MinionCardClass || source.CurrentCard.Class is AvatarCardClass)
            {
                if (target.CurrentCard.Class is MinionCardClass || target.CurrentCard.Class is AvatarCardClass)
                {
                    ICardStat sourceAttack = source.CurrentCard.Class.Stats[DefaultStats.Power];
                    ICardStat sourceHealth = source.CurrentCard.Class.Stats[DefaultStats.Health];
                    ICardStat targetAttack = target.CurrentCard.Class.Stats[DefaultStats.Power];
                    ICardStat targetHealth = target.CurrentCard.Class.Stats[DefaultStats.Health];

                    targetHealth.Value = targetHealth.Value - sourceAttack.Value;
                    sourceHealth.Value = sourceHealth.Value - targetAttack.Value;

                    if (targetHealth.Value > 0) result.Add(ServerAttackResult.TargetSurvived);
                    else if (target.IsAvatar) result.Add(ServerAttackResult.TargetAvatarKilled);
                    if (sourceHealth.Value > 0) result.Add(ServerAttackResult.SourceSurvived);
                    else if (source.IsAvatar) result.Add(ServerAttackResult.SourceAvatarKilled); //todo really we can move it?
                }
            }

            return result;
        }
    }

    public enum ServerAttackResult
    {
        TargetSurvived,
        SourceSurvived,
        SourceAvatarKilled,
        TargetAvatarKilled
    }

    public enum ServerTurnActionError
    {
        DoesNotExist,
        MoveDeniedOccupied,
        UnmoveableCard,
        CardMissing
    }

    public static class SimpleTurnManager
    {
        public static HashSet<SimpleMoveResult> ProcessCard(ICardView asource, ICardView atarget, out Action reverseAction)
        {
            HashSet<SimpleMoveResult> result = new HashSet<SimpleMoveResult>();
            reverseAction = null;
            var source = asource.ContextModel.ContentCard;
            var target = atarget.ContextModel.ContentCard;
            if (source.Class is MinionCardClass || source.Class is AvatarCardClass)
            {
                if (target.Class is MinionCardClass || target.Class is AvatarCardClass)
                {
                    source.Class.Stats.TryGetValue(DefaultStats.Power, out ICardStat sourceAttack);
                    source.Class.Stats.TryGetValue(DefaultStats.Health, out ICardStat sourceHealth);
                    target.Class.Stats.TryGetValue(DefaultStats.Power, out ICardStat targetAttack);
                    target.Class.Stats.TryGetValue(DefaultStats.Health, out ICardStat targetHealth);

                    targetHealth.Value = targetHealth.Value - sourceAttack.Value;
                    sourceHealth.Value = sourceHealth.Value - targetAttack.Value;

                    asource.ContextModel.SetCard(source);
                    atarget.ContextModel.SetCard(target);

                    if (targetHealth.Value > 0) result.Add(SimpleMoveResult.TargetSurvived);
                    if (sourceHealth.Value > 0) result.Add(SimpleMoveResult.SourceSurvived);

                    reverseAction = (() =>
                    {
                        targetHealth.Value = targetHealth.Value + sourceAttack.Value;
                        sourceHealth.Value = sourceHealth.Value + targetAttack.Value;

                        asource.ContextModel.SetCard(source);
                        atarget.ContextModel.SetCard(target);
                    });
                }
            }
            return result;
        }
    }
}