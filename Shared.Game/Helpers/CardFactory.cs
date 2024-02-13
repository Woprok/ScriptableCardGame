using Shared.Game.Controls;
using Shared.Game.Entities.Cards.Interfaces;
using Shared.Game.Views;

namespace Shared.Game.Helpers
{
    /// <summary>
    /// Used by GameBoardGrid to create cards
    /// </summary>
    public interface ICardFactory
    {
        int CardWidth { get; set; }
        int CardHeight { get; set; }

        MoveableCardHost CreateCard(ICardView card);
        MoveableCardHost CreateCard(ICard card);
    }

    public class CardFactory : ICardFactory
    {
        public CardFactory(int cardWidth, int cardHeight)
        {
            CardWidth = cardWidth;
            CardHeight = cardHeight;
        }

        public int CardWidth { get; set; }
        public int CardHeight { get; set; }

        public MoveableCardHost CreateCard(ICardView card)
        {
            return new MoveableCardHost(card)
            {
                Width = CardWidth,
                Height = CardHeight,
            };
        }

        public MoveableCardHost CreateCard(ICard card)
        {
            return CreateCard(new CardView(card));
        }
    }
}