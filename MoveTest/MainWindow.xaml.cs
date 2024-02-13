using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Shared.Game.Controls;
using Shared.Game.Entities;
using Shared.Game.Entities.Cards.CardClasses;
using Shared.Game.Entities.Cards.Default;
using Shared.Game.Helpers;
using Shared.Game.Views;

namespace MoveTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameBoardGrid gameBoardGrid;
        public MainWindow()
        {
            InitializeComponent();
            
            //Test Factory
            ICardHolderFactory cardHolderFactory = new CardHolderFactory(150, 150);
            IGridFactory gridFactory = new GridFactory();
            IGameBoardFactory gameFactory = new GameBoardFactory(gridFactory, cardHolderFactory, new CardFactory(100,100), new PlayerFactory(gridFactory, cardHolderFactory));

            //TestPlayer
            Player enem1 = new Player(Guid.NewGuid(), "Brunet");
            Player player = new Player(Guid.NewGuid(), "Dasta");
            HashSet<Player> enemyPlayers = new HashSet<Player>()
            {
                enem1,
                new Player(Guid.NewGuid(), "Ainz"),
                new Player(Guid.NewGuid(), "EdgeLord49532")
            };

            //Finally Define gameBoard
            gameBoardGrid = gameFactory.CreateGameBoard(player,null,enemyPlayers,3,3);

            GridParent.Content = gameBoardGrid;
            
            //Try new card
            var first = new Card(enem1.Account, "Ferdinand", true);
            first.Class = new AvatarCardClass(12,12);
            var second = new Card(player.Account, "Jeremiash", true);

            gameBoardGrid.PlaceCard(enem1, 1, 1, new CardView(first), true);
            gameBoardGrid.PlaceCard(player, 2, 1, new CardView(second), true);

            //newFunction

            foreach (KeyValuePair<Player, CardHolderBorder> avatarHolder in gameBoardGrid.AvatarHolders)
            {
                gameBoardGrid.PlaceCard(
                    avatarHolder.Key, 
                    avatarHolder.Value.LogicalRow, 
                    avatarHolder.Value.LogicalColumn, new CardView(avatarHolder.Key.Avatar.AvatarCard),
                    true);
            }


        }


    }
}