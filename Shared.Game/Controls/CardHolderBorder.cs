using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Shared.Game.Entities;
using Shared.Game.Views;

namespace Shared.Game.Controls
{
    public sealed class CardHolderBorder : Border
    {
        public CardHolderBorder(Player player, int logicalRow, int logicalColumn, bool isOwnedByPlayer) : base()
        {
            Player = player;
            LogicalRow = logicalRow;
            LogicalColumn = logicalColumn;
            IsOwnedByPlayer = isOwnedByPlayer;
        }

        public Player Player { get; set; }
        /// <summary>
        /// Determines if this Holder is part of player board, or it's part of his user interface.
        /// True = owned, False = part of UI.
        /// </summary>
        public bool IsOwnedByPlayer { get; private set; }
        public int LogicalRow { get; set; }
        public int LogicalColumn { get; set; }
        
        public bool HasChild { get { return this.Child != null; } }
        public MoveableCardHost GetCardHost { get { return this.Child as MoveableCardHost; } }
        public ICardView GetCardView { get { return GetCardHost.DisplayedCard; } }
    }
}