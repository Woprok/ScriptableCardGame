using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Shared.Game.Controls;
using Shared.Game.Entities;
using Shared.Game.Views;

namespace Shared.Game.Helpers
{
    public interface IGameBoardFactory
    {
        int EnemyRowCount { get; }
        int FriendlyRowCount { get; }
        int PlayerRowCount { get; }
        int TotalRowCount { get; }
        IGridFactory GridFactory { get; }
        ICardHolderFactory CardHolderFactory { get; }
        ICardFactory CardFactory { get; }
        IPlayerFactory PlayerFactory { get; }
        GameBoardGrid CreateGameBoard(Player thisPlayer, HashSet<Player> friendlyPlayers, HashSet<Player> enemyPlayers, int rowCount, int columnCount);
        ZoneGrid CreatePlayersZone(GameBoardGrid gameBoard, HashSet<Player> players, int playerRowCount, int rowCount, int columnCount);
        ZoneGrid CreatePlayerZone(GameBoardGrid gameBoard, Player player, int rowCount, int columnCount);
    }

    public class GameBoardFactory : IGameBoardFactory
    {
        /// <summary>
        /// intruduces extremly limited gameBoard
        /// </summary>
        public GameBoardFactory(IGridFactory gridFactory, ICardHolderFactory cardHolderFactory, ICardFactory cardFactory, IPlayerFactory playerFactory)
        {
            GridFactory = gridFactory;
            CardHolderFactory = cardHolderFactory;
            CardFactory = cardFactory;
            PlayerFactory = playerFactory;
        }

        /// <summary>
        /// Define number of rows int which are opponets split
        /// ToDo implement working generator
        /// </summary>
        public int EnemyRowCount { get; } = 1;
        /// <summary>
        /// If team mechanic implemented
        /// ToDo implement teams
        /// </summary>
        public int FriendlyRowCount { get; } = 0;
        /// <summary>
        /// this is here just for reference
        /// </summary>
        public int PlayerRowCount { get; } = 1;
        public int TotalRowCount { get => EnemyRowCount + FriendlyRowCount + PlayerRowCount; }
        public IGridFactory GridFactory { get; }
        public ICardHolderFactory CardHolderFactory { get; }
        public ICardFactory CardFactory { get; }
        public IPlayerFactory PlayerFactory { get; }

        /// <summary>
        /// Implements simple player grid for ffa game, enemyRow is scrollable
        /// </summary>
        public GameBoardGrid CreateGameBoard(Player thisPlayer, HashSet<Player> friendlyPlayers, HashSet<Player> enemyPlayers, int rowCount, int columnCount)
        {
            GameBoardGrid gameBoard = new GameBoardGrid(CardFactory, thisPlayer);

            GridFactory.DefineNewRows(gameBoard, TotalRowCount);

            //Enemy
            var enemyPlayerZone = CreatePlayersZone(gameBoard, enemyPlayers, EnemyRowCount, rowCount, columnCount);

            ScrollViewer enemyScrollBar = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = enemyPlayerZone
            };
            Grid.SetRow(enemyScrollBar, 0); //todo logical row position

            gameBoard.Children.Add(enemyScrollBar);

            //Friendly
            //ToDo implement teams
            //todo logical row position

            //Player
            ZoneGrid playerZone = CreatePlayerZone(gameBoard, thisPlayer, rowCount, columnCount);
            Grid.SetRow(playerZone, 1); //todo logical row position

            gameBoard.Children.Add(playerZone);
            
            return gameBoard;
        }

        //ToDo implement teams
        public ZoneGrid CreatePlayersZone(GameBoardGrid gameBoard, HashSet<Player> players, int playerRowCount, int rowCount, int columnCount)
        {
            ZoneGrid zoneGrid = new ZoneGrid(players);

            GridFactory.DefineNewRows(zoneGrid, playerRowCount);
            GridFactory.DefineNewColumns(zoneGrid, players.Count); //todo implement working split

            int currentRow = 0;
            int currentColumn = 0;
            foreach (Player player in players)
            {
                var playerBoard = PlayerFactory.CreatePlayerBoard(gameBoard, player, rowCount, columnCount);

                Grid.SetRow(playerBoard, currentRow);
                Grid.SetColumn(playerBoard, currentColumn);

                zoneGrid.Children.Add(playerBoard);

                currentColumn++;
                if (currentColumn == players.Count) //todo implement working split
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }

            return zoneGrid;
        }

        public ZoneGrid CreatePlayerZone(GameBoardGrid gameBoard, Player player, int rowCount, int columnCount)
        {
            ZoneGrid zoneGrid = new ZoneGrid(new HashSet<Player>(){ player });

            GridFactory.DefineNewRows(zoneGrid, PlayerRowCount);
            GridFactory.DefineNewColumns(zoneGrid, 2);

            //Define this player board
            PlayerBoardGrid playerBoard = PlayerFactory.CreatePlayerBoard(gameBoard, player, rowCount, columnCount);

            Grid.SetRow(playerBoard, 0);
            Grid.SetColumn(playerBoard, 0);

            zoneGrid.Children.Add(playerBoard);

            //Define this player cardCreator
            var fullCardCreator = CreateFullCardCreator(gameBoard, player);

            Grid.SetRow(fullCardCreator, 0);
            Grid.SetColumn(fullCardCreator, 1);

            zoneGrid.Children.Add(fullCardCreator);

            return zoneGrid;
        }

        public Grid CreateFullCardCreator(GameBoardGrid gameBoard, Player player, int defaultNewRow = 0, int defaultNewColumn = 0)
        {
            Grid cardEditor = new Grid();
            GridFactory.DefineNewColumns(cardEditor, 2);

            //CardHolder for CardCreator
            CardHolderBorder cardHolder = CardHolderFactory.CreateCardCreatorHolder(player, defaultNewRow, defaultNewColumn);
            cardHolder.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(cardHolder, 0);

            gameBoard.NewCardHolder = cardHolder;

            gameBoard.CardHolders.Add(cardHolder);
            cardEditor.Children.Add(cardHolder);

            //CardCreator
            ContentControl control = new ContentControl();
            ICardCreatorView cardCreator = new CardCreatorView(player);

            cardCreator.ContextModel.NewCardCreated += gameBoard.OnNewCardCreated;

            control.Content = cardCreator;
            Grid.SetColumn(control, 1);

            cardEditor.Children.Add(control);

            return cardEditor;
        }
    }
}