using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shared.Common.Languages;
using Shared.Game.Entities;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Entities.Turns;
using Shared.Game.Helpers;
using Shared.Game.Views;

namespace Shared.Game.Controls
{
    /// <summary>
    /// Grid responsible for handling game mechanic of moving cards
    /// </summary>
    public class GameBoardGrid : Grid
    {
        public GameBoardGrid(ICardFactory cardFactory, Player thisPlayer)
        {
            CardFactory = cardFactory;
            ThisPlayer = thisPlayer;
        }

        public ICardFactory CardFactory { get; }

        /// <summary>
        /// Originally used in MouseLeave
        /// </summary>
        private FrameworkElement ObtainParentElement { get => this.Parent as FrameworkElement;}

        public CardHolderBorder NewCardHolder { get; set; }
        public HashSet<CardHolderBorder> CardHolders { get; set; } = new HashSet<CardHolderBorder>();
        public Dictionary<Player, CardHolderBorder> AvatarHolders { get; set; } = new Dictionary<Player, CardHolderBorder>();
        public HashSet<MoveableCardHost> CardHosts { get; set; } = new HashSet<MoveableCardHost>();

        public Player ThisPlayer { get; set; }
        public bool CanPlay { get; set; } = false;

        public void AddMoveableHost(MoveableCardHost moveableCardHost)
        {
            CardHosts.Add(moveableCardHost);
            moveableCardHost.OnMovementEnd += this.CorrectPositionOnMovementEnd;
            //Originally Parent of this grid
            this.MouseLeave += moveableCardHost.OnMouseLeftWindow;
            this.SizeChanged += moveableCardHost.OnParentSizeChanged;
        }

        public void RemoveMoveableHost(MoveableCardHost moveableCardHost)
        {
            moveableCardHost.OnMovementEnd -= this.CorrectPositionOnMovementEnd;
            //Originally Parent of this grid
            this.MouseLeave -= moveableCardHost.OnMouseLeftWindow;
            this.SizeChanged -= moveableCardHost.OnParentSizeChanged;
        }

        /// <summary>
        /// This will place specific card at specific position of specified player.
        /// IsOwned indicates if the target place is owned by player
        /// </summary>
        public bool PlaceCard(Player player, int row, int column, CardView card, bool isOwned)
        {
            CardHolderBorder initialHolder = CardHolders.FirstOrDefault(item => item.Player.Account == player.Account
                                                                       && item.IsOwnedByPlayer == isOwned
                                                                       && item.LogicalRow == row 
                                                                       && item.LogicalColumn == column);
            if (initialHolder == null || initialHolder.HasChild)
                return false;

            MoveableCardHost newCard = CardFactory.CreateCard(card);
            
            AddMoveableHost(newCard);

            //Set card to found holder
            newCard.SetToPlace(this, initialHolder);

            return true;
        }

        public void OnNewCardCreated(object sender, ICard card)
        {
            if (NewCardHolder.HasChild)
            {
                var result = MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(GameBoardGrid), "Do you wish to destroy old card ?"),
                    LanguageHelper.TranslateContextual(nameof(GameBoardGrid), "Card will be destroyed."),
                    MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                if (result == MessageBoxResult.OK)
                    NewCardHolder.GetCardHost.DestroyMoveableHost();
                else
                {
                    return;
                }
            }

            MoveableCardHost newCard = CardFactory.CreateCard(card);

            AddMoveableHost(newCard);

            //Set card to found holder
            newCard.SetToPlace(this, NewCardHolder);
        }

        public void CorrectPositionOnMovementEnd(MoveableCardHost moveableCard)
        {
            reverseAction = null;
            Point centeringPoint;
            CardHolderBorder foundClosestHolder = moveableCard.FindClosestHolder(out centeringPoint);

            //this is considered cancellation of moving
            if (foundClosestHolder == null)
            {
                moveableCard.ReturnToLastHolder();
                return;
            }

            CanPlay = false;
            ForcedMove(moveableCard, centeringPoint, foundClosestHolder);
        }

        private void ForcedMove(MoveableCardHost moveableCard, Point centeringPoint, CardHolderBorder foundClosestHolder)
        {
            //this is moving to someone else
            if (foundClosestHolder.Child != null && !ReferenceEquals(foundClosestHolder, moveableCard.LastCardHolder))
            {
                if (moveableCard.LastCardHolder.IsOwnedByPlayer == false)
                {
                    MessageBox.Show(
                        LanguageHelper.TranslateContextual(nameof(GameBoardGrid), "Cant attack from creation!"),
                        LanguageHelper.TranslateContextual(nameof(GameBoardGrid),
                            "Sorry this will be in next version!"),
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                    moveableCard.ReturnToLastHolder();
                    CanPlay = true;
                    return;
                }

                var moveResults = SimpleTurnManager.ProcessCard(moveableCard.DisplayedCard, foundClosestHolder.GetCardView, out reverseAction);

                SetTurnAttackAction(moveableCard, moveableCard.LastCardHolder, foundClosestHolder, moveResults);

                if (!moveResults.Contains(SimpleMoveResult.TargetSurvived))
                {
                    foundClosestHolder.GetCardHost.DestroyMoveableHost();
                }

                if (!moveResults.Contains(SimpleMoveResult.SourceSurvived))
                {
                    moveableCard.DestroyMoveableHost();
                    return;
                }

                if (moveResults.Contains(SimpleMoveResult.SourceSurvived) &&
                    moveResults.Contains(SimpleMoveResult.TargetSurvived))
                {
                    moveableCard.ReturnToLastHolder();
                    return;
                }
            }

            SetTurnMoveAction(moveableCard, moveableCard.LastCardHolder, foundClosestHolder);
            moveableCard.MoveToHolder(foundClosestHolder, centeringPoint);
        }

        private Action reverseAction = null;
        private TurnAction lastTurnAction;

        private void SetTurnAttackAction(MoveableCardHost movedHost, CardHolderBorder sourceBorder, CardHolderBorder targetBorder, HashSet<SimpleMoveResult> results)
        {
            var source = new CardInfo(sourceBorder.Player, movedHost.DisplayedCard.ContextModel.ContentCard, sourceBorder.LogicalRow, sourceBorder.LogicalColumn, sourceBorder.IsOwnedByPlayer);
            var target = new CardInfo(targetBorder.Player, targetBorder.GetCardView.ContextModel.ContentCard, targetBorder.LogicalRow, targetBorder.LogicalColumn, null);
            lastTurnAction = new TurnAction(new AttackAction(source,target,results));
        }

        private void SetTurnMoveAction(MoveableCardHost movedHost, CardHolderBorder sourceBorder, CardHolderBorder targetBorder)
        {
            var source = new CardInfo(sourceBorder.Player, movedHost.DisplayedCard.ContextModel.ContentCard, sourceBorder.LogicalRow, sourceBorder.LogicalColumn, sourceBorder.IsOwnedByPlayer);
            var target = new CardInfo(targetBorder.Player, null, targetBorder.LogicalRow, targetBorder.LogicalColumn, null);
            lastTurnAction = new TurnAction(new MoveAction(source, target));
        }

        public TurnAction GetLastTurn()
        {
            return lastTurnAction;
        }

        public void PlayThis(CardAction playedAction)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MoveableCardHost cardToMove = null;
                CardHolderBorder targetBorder = null;
                if (playedAction is MoveAction move)
                {
                    if (move.SourceCard.IsNewCard == false)
                    {
                        PlaceCard(move.TargetPosition.Player, move.TargetPosition.Row, move.TargetPosition.Column,
                            new CardView(move.SourceCard.Card), true);
                        return;
                    }
                    else
                    {
                        CardHolderBorder source = CardHolders.First(item =>
                            Equals(item.Player, move.SourceCard.Player) && item.LogicalRow == move.SourceCard.Row
                                                                        && item.LogicalColumn == move.SourceCard.Column);

                        PlaceCard(move.SourceCard.Player, move.SourceCard.Row, move.SourceCard.Column,
                            new CardView(move.SourceCard.Card), true);

                        cardToMove = source.GetCardHost;

                        targetBorder = CardHolders.First(item =>
                            Equals(item.Player, move.TargetPosition.Player) && item.LogicalRow == move.TargetPosition.Row
                                                                            && item.LogicalColumn ==
                                                                            move.TargetPosition.Column);
                    }
                }

                if (playedAction is AttackAction attack)
                {
                    if (attack.SourceCard.IsNewCard == false)
                    {
                        return;
                    }
                    else
                    {
                        CardHolderBorder source = CardHolders.First(item =>
                            Equals(item.Player, attack.SourceCard.Player) && item.LogicalRow == attack.SourceCard.Row
                                                                          && item.LogicalColumn ==
                                                                          attack.SourceCard.Column);

                        PlaceCard(attack.SourceCard.Player, attack.SourceCard.Row, attack.SourceCard.Column,
                            new CardView(attack.SourceCard.Card), true);

                        cardToMove = source.GetCardHost;

                        targetBorder = CardHolders.First(item =>
                            Equals(item.Player, attack.TargetCard.Player) && item.LogicalRow == attack.TargetCard.Row
                                                                          && item.LogicalColumn ==
                                                                          attack.TargetCard.Column);
                    }
                }

                if (cardToMove == null || targetBorder == null)
                    return;

                ForcedMove(cardToMove, targetBorder.TranslatePoint(new Point(0, 0), cardToMove), targetBorder);
            });
        }

        public void ReverseLastTurn()
        {
            Application.Current.Dispatcher.InvokeAsync(() => {
                reverseAction?.Invoke();
                reverseAction = null;
                lastTurnAction = null;
            });
        }

        public void ClearLastTurn()
        {
            reverseAction = null;
            lastTurnAction = null;
        }
    }
}