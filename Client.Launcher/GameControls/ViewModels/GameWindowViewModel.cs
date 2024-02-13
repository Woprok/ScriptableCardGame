using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Client.Launcher.GameControls.Interfaces;
using Client.Launcher.GameControls.Views;
using Client.Launcher.ViewModels;
using Shared.Common.Languages;
using Shared.Common.Models;
using Shared.Game.Entities;
using Shared.Game.Entities.Turns;
using Shared.Game.ViewModels;
using Shared.Game.Views;
using Shared.Networking.Game.Entities;
using Shared.Networking.Game.Managers;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Game.Messages.Responses;
using Shared.Networking.Game.Messages.Updates;
using Shared.Networking.Protocol.Entities;

namespace Client.Launcher.GameControls.ViewModels
{
    /// <summary>
    /// Should never be reused, for new game create new window be wastefull!
    /// </summary>
    public class GameWindowViewModel : WindowViewModelBase, IGameWindow
    {
        private ObservableCollection<AccountEntity> accountList = new ObservableCollection<AccountEntity>();
        private AccountEntity selectedAccount;
        private GameBoardView gameField;
        private bool canPlay;
        private string currentPlayerValue;

        public Guid? GameInstanceId { get; private set; }
        public GameClientManager GameManager { get; private set; }
        public void SetGameManager(Guid gameId, GameClientManager gameManager)
        {
            GameInstanceId = gameId;
            GameManager = gameManager;
            GameManager.OnGameFinished += OnGameFinished; 
            GameManager.OnGameStateStart += OnGameStateStart; 
            GameManager.OnGamePlayersUpdated += OnGamePlayersUpdated; 
            GameManager.OnGameStateUpdated += OnGameStateUpdated; 
            GameManager.OnProcessTurn += OnProcessTurn; 
            GameField = new GameBoardView();
        }

        public void OnUserCloseWindow()
        {
            GameManager.SendLeaveGameUpdate(GameInstanceId.Value);
        }

        public void OnActivate()
        {
            GameManager.SendJoinGameResponse(GameInstanceId.Value);
        }

        private void OnGameStateStart(object sender, ClientGameInstance e)
        {
            Player thisPlayer = e.GameState.PlayerCollection.Values.First(item => item.Account == e.CurrentAccount.Id);
            HashSet<Player> enemyPlayers = e.GameState.PlayerCollection.Values.Where(item => item.Account != e.CurrentAccount.Id).ToHashSet();

            GameField.ContextModel.SetNewGameBoard(thisPlayer, null, enemyPlayers, e.GameState.GameBoardSettings);

            ProcessNextPlayer(e, e.GameState.CurrentPlayer);
        }

        private void OnGameStateUpdated(object sender, Shared.Networking.Game.Entities.ClientGameInstance e)
        {
            AccountList = new ObservableCollection<AccountEntity>(e.Instance.ConnectedPlayers);

            ProcessNextPlayer(e, e.GameState.CurrentPlayer);
        }

        private void OnGamePlayersUpdated(object sender, Shared.Networking.Game.Entities.GameInstanceEntity e)
        {
            AccountList = new ObservableCollection<AccountEntity>(e.ConnectedPlayers);
        }

        private void OnGameFinished(object sender, Shared.Networking.Game.Entities.ClientGameInstance e)
        {
            AccountList = new ObservableCollection<AccountEntity>(e.Instance.ConnectedPlayers);

            ProcessNextPlayer(e, e.GameState.CurrentPlayer);
        }

        public void ProcessTurnResponse(GameResponseMessage responseMessage)
        {
            if (responseMessage is ProcessTurnResponse response)
            {
                if (response.ActionResponse == TurnActionResponse.ValidMove)
                {
                    OnSuccessfullMovement(response);
                }
                else
                {
                    OnFailedMovement(response);
                }
            }
        }
        private void OnProcessTurn(object sender, Shared.Networking.Game.Messages.Updates.ProcessTurnUpdate update)
        {
            if (update.Action.UsedAction)
            {
                if (update.Action.PlayedAction.GetType() == typeof(MoveAction))
                {
                    GameField.ContextModel.ProcessAction(update.Action.PlayedAction);
                }

                if (update.Action.PlayedAction.GetType() == typeof(AttackAction))
                {
                    GameField.ContextModel.ProcessAction(update.Action.PlayedAction);
                }
            }

            ProcessNextPlayer(update.Instance, update.NextPlayer);
        }

        public void ProcessNextPlayer(Guid instanceId, int nextPlayer)
        {
            ClientGameInstance gameInstance = GameManager.GetCurrentGameInstance(instanceId);

            InternalProcessNextPlayer(nextPlayer, gameInstance);
        }
        public void ProcessNextPlayer(ClientGameInstance gameInstance, int nextPlayer)
        {
            InternalProcessNextPlayer(nextPlayer, gameInstance);
        }

        private void InternalProcessNextPlayer(int nextPlayer, ClientGameInstance gameInstance)
        {
            gameInstance.GameState.CurrentPlayer = nextPlayer;
            if (gameInstance.GameState.PlayerCollection[nextPlayer].Account == gameInstance.CurrentAccount.Id)
            {
                CurrentPlayerValue = LanguageHelper.TranslateContextual(nameof(GameWindowViewModel), "Your glorious turn!");
                GameField.ContextModel.ProcessNextPlayer(true);
                CanPlay = true;
            }
            else
            {
                CurrentPlayerValue = gameInstance.GameState.PlayerCollection[nextPlayer].Account.ToString();
                GameField.ContextModel.ProcessNextPlayer(false);
                CanPlay = false;
            }
            GameField.ContextModel.ClearLastTurn();
        }

        /// <summary>
        /// Happens if card action was accepted
        /// </summary>
        public void OnSuccessfullMovement(ProcessTurnResponse response)
        {
            //Be happy
            ProcessNextPlayer(response.Instance, response.NextPlayer);
            //Move to next player
        }

        /// <summary>
        /// Happens if card action is not legal
        /// </summary>
        public void OnFailedMovement(ProcessTurnResponse response)
        {
            MessageBox.Show(
                LanguageHelper.TranslateContextual(nameof(GameWindowViewModel), "Illegal action error!"),
                LanguageHelper.TranslateContextual(nameof(GameWindowViewModel), "You cheated!"),
                MessageBoxButton.OK, MessageBoxImage.Stop);

            GameField.ContextModel.ReverseLastTurn();
            
            GameField.ContextModel.ProcessNextPlayer(true);
            CanPlay = true;

            GameField.ContextModel.ClearLastTurn();
        }

        public void NextTurn()
        {
            GameField.ContextModel.ProcessNextPlayer(false);
            CanPlay = false;

            var turn = new TurnAction() {UsedAction = false};
            if (GameField.ContextModel.GetLastTurnAction() != null)
            {
                turn = GameField.ContextModel.GetLastTurnAction();
            }

            GameManager.SendProcessTurnRequest(GameInstanceId.Value, turn
                , ProcessTurnResponse);
        }

        public ObservableCollection<AccountEntity> AccountList
        {
            get => accountList;
            set
            {
                if (Equals(value, accountList)) return;
                accountList = value;
                OnPropertyChanged();
            }
        }
        public AccountEntity SelectedAccount
        {
            get => selectedAccount;
            set
            {
                if (Equals(value, selectedAccount)) return;
                selectedAccount = value;
                OnPropertyChanged();
            }
        }
        public GameBoardView GameField
        {
            get => gameField;
            set
            {
                if (Equals(value, gameField)) return;
                gameField = value;
                OnPropertyChanged();
            }
        }
        public bool CanPlay
        {
            get => canPlay;
            set
            {
                if (canPlay == value) return;
                canPlay = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPlayerValue
        {
            get => currentPlayerValue;
            set
            {
                if (currentPlayerValue == value) return;
                currentPlayerValue = value; 
                OnPropertyChanged();
            }
        }
    }
}