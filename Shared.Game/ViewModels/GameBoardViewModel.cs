using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Windows;
using Shared.Common.Models;
using Shared.Game.Controls;
using Shared.Game.Entities;
using Shared.Game.Entities.GameBoards;
using Shared.Game.Entities.Turns;
using Shared.Game.Helpers;
using Shared.Game.Views;

namespace Shared.Game.ViewModels
{
    public interface IGameBoardViewModel
    {
        void BuildFactoriesFrom(ICardHolderFactory cardHolderFactory, IGridFactory gridFactory, ICardFactory cardFactory, IPlayerFactory playerFactory, IGameBoardFactory gameBoardFactory);
        void SetNewGameBoard(Player player, HashSet<Player> friendlyPlayers, HashSet<Player> enemyPlayers, GameBoardSettings settings);
        GameBoardGrid GameBoard { get; set; }
        ICardHolderFactory CardHolderFactory { get; }
        IGridFactory GridFactory { get; }
        ICardFactory CardFactory { get; }
        IPlayerFactory PlayerFactory { get; }
        IGameBoardFactory GameFactory { get; }
        void ProcessNextPlayer(bool canPlay);
        TurnAction GetLastTurnAction();
        void ProcessAction(CardAction playedAction);
        void ReverseLastTurn();
        void ClearLastTurn();
    }

    public class GameBoardViewModel : ViewModelBase, IGameBoardViewModel
    {
        private GameBoardGrid gameBoard;

        public GameBoardViewModel()
        {
            PlayerFactory = new PlayerFactory(GridFactory, CardHolderFactory);
            GameFactory = new GameBoardFactory(GridFactory, CardHolderFactory, CardFactory, PlayerFactory);
        }

        public void SetNewGameBoard(Player player, HashSet<Player> friendlyPlayers, HashSet<Player> enemyPlayers, GameBoardSettings settings)
        {
            Application.Current.Dispatcher.Invoke(() => {
                GameBoard = GameFactory.CreateGameBoard(player, friendlyPlayers, enemyPlayers, settings.RowCount, settings.ColumnCount);
                SetDefaultPlayers();
            });
        }

        public void SetDefaultPlayers()
        {
            foreach (KeyValuePair<Player, CardHolderBorder> avatarHolder in GameBoard.AvatarHolders)
            {
                GameBoard.PlaceCard(
                    avatarHolder.Key,
                    avatarHolder.Value.LogicalRow,
                    avatarHolder.Value.LogicalColumn, new CardView(avatarHolder.Key.Avatar.AvatarCard),
                    true);
            }
        }

        public void BuildFactoriesFrom(ICardHolderFactory cardHolderFactory, IGridFactory gridFactory, ICardFactory cardFactory, IPlayerFactory playerFactory, IGameBoardFactory gameBoardFactory)
        {
            CardHolderFactory = cardHolderFactory;
            GridFactory = gridFactory;
            CardFactory = cardFactory;
            PlayerFactory = playerFactory;
            GameFactory = gameBoardFactory;
        }

        public GameBoardGrid GameBoard
        {
            get => gameBoard;
            set
            {
                if (Equals(gameBoard, value)) return;
                gameBoard = value; 
                OnPropertyChanged();
            }
        }

        public ICardHolderFactory CardHolderFactory { get; private set; } = new CardHolderFactory(150, 150);
        public IGridFactory GridFactory { get; private set; } = new GridFactory();
        public ICardFactory CardFactory { get; private set; } = new CardFactory(100, 100);
        public IPlayerFactory PlayerFactory { get; private set; }
        public IGameBoardFactory GameFactory { get; private set; }
        public void ProcessNextPlayer(bool canPlay)
        {
            GameBoard.CanPlay = canPlay;
        }

        public TurnAction GetLastTurnAction()
        {
            return GameBoard.GetLastTurn();
        }

        public void ProcessAction(CardAction playedAction)
        {
            if (GetLastTurnAction() == null || playedAction.ActionId != GetLastTurnAction().PlayedAction.ActionId)
                GameBoard.PlayThis(playedAction);
        }

        public void ReverseLastTurn()
        {
            GameBoard.ReverseLastTurn();
        }

        public void ClearLastTurn()
        {
            GameBoard.ClearLastTurn();
        }
    }
}