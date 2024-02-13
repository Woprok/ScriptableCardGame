using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shared.Game.Controls;
using Shared.Game.Entities;

namespace Shared.Game.Helpers
{
    public interface IPlayerFactory
    {
        IGridFactory GridFactory { get; }
        ICardHolderFactory CardHolderFactory { get; }
        PlayerBoardGrid CreatePlayerBoard(GameBoardGrid gameBoard, Player player, int rows, int columns);
    }

    public class PlayerFactory : IPlayerFactory
    {
        public PlayerFactory(IGridFactory gridFactory, ICardHolderFactory cardHolderFactory)
        {
            GridFactory = gridFactory;
            CardHolderFactory = cardHolderFactory;
        }

        public IGridFactory GridFactory { get; }
        public ICardHolderFactory CardHolderFactory { get; }

        /// <summary>
        /// Initialize PlayerBoard with AvatarHolder
        /// </summary>
        public PlayerBoardGrid CreatePlayerBoard(GameBoardGrid gameBoard, Player player, int rowCount, int columnCount)
        {
            PlayerBoardGrid playerBoard = new PlayerBoardGrid(player);

            GridFactory.DefineNewRows(playerBoard, rowCount);
            GridFactory.DefineNewColumns(playerBoard, columnCount + 1);

            //Set avatarHolder
            int avatarRow = rowCount / 2;
            int avatarColumn = 0;
            CardHolderBorder avatarHolder = CardHolderFactory.CreateAvatarHolder(player, avatarRow, avatarColumn);

            Grid.SetRow(avatarHolder, rowCount / 2);
            Grid.SetColumn(avatarHolder, 0);

            gameBoard.AvatarHolders.Add(player, avatarHolder);
            gameBoard.CardHolders.Add(avatarHolder);
            playerBoard.Children.Add(avatarHolder);

            //Set cardHolders
            for (int cardRow = 0; cardRow < rowCount; cardRow++)
            {
                for (int cardColumn = 1; cardColumn < columnCount + 1; cardColumn++)
                {
                    CardHolderBorder cardHolder = CardHolderFactory.CreateCardHolder(player, cardRow, cardColumn);

                    Grid.SetRow(cardHolder, cardRow);
                    Grid.SetColumn(cardHolder, cardColumn);

                    gameBoard.CardHolders.Add(cardHolder);
                    playerBoard.Children.Add(cardHolder);
                }
            }

            return playerBoard;
        }
    }
}