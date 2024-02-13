using System.Windows;
using System.Windows.Media;
using Shared.Game.Controls;
using Shared.Game.Entities;

namespace Shared.Game.Helpers
{
    public interface ICardHolderFactory
    {
        int CardHolderWidth { get; set; }
        int CardHolderHeight { get; set; }
        CardHolderBorder CreateCardHolder(Player player, int row, int column);
        CardHolderBorder CreateAvatarHolder(Player player, int row, int column);
        CardHolderBorder CreateCardCreatorHolder(Player player, int row, int column);
    }

    public class CardHolderFactory : ICardHolderFactory
    {
        public CardHolderFactory(int cardHolderWidth, int cardHolderHeight)
        {
            CardHolderWidth = cardHolderWidth;
            CardHolderHeight = cardHolderHeight;
        }

        public int CardHolderWidth { get; set; }
        public int CardHolderHeight { get; set; }

        public CardHolderBorder CreateCardHolder(Player player, int row, int column)
        {
            return new CardHolderBorder(player, row, column, true)
            {
                Width = CardHolderWidth,
                Height = CardHolderHeight,
                Background = Brushes.LightSkyBlue,
                Margin = new Thickness(1)
            };
        }

        public CardHolderBorder CreateAvatarHolder(Player player, int row, int column)
        {
            return new CardHolderBorder(player, row, column, true)
            {
                Width = CardHolderWidth,
                Height = CardHolderHeight,
                Background = Brushes.DeepSkyBlue,
                Margin = new Thickness(2)
            };
        }

        public CardHolderBorder CreateCardCreatorHolder(Player player, int row, int column)
        {
            return new CardHolderBorder(player, row, column, false)
            {
                Width = CardHolderWidth,
                Height = CardHolderHeight,
                Background = Brushes.PaleVioletRed,
                Margin = new Thickness(5)
            };
        }
    }
}