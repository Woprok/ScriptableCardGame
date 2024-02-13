using System;
using System.Collections.ObjectModel;
using System.Windows;
using Client.Launcher.GameControls.Views;
using Client.Launcher.Interfaces;
using Client.Launcher.Views;
using Shared.Common.Enums;
using Shared.Common.Languages;
using Shared.Common.Models;
using Shared.Common.Views;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Game.Messages.Base;
using Shared.Networking.Game.Messages.Responses;
using Shared.Networking.Game.Messages.Updates;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Enums;
using Shared.Networking.Protocol.Models;

namespace Client.Launcher.ViewModels
{
    public sealed class MenuWindowViewModel : WindowViewModelBase, IMenuWindow
    {
        private ObservableCollection<LobbyEntity> lobbyList = new ObservableCollection<LobbyEntity>();
        private LobbyEntity selectedLobby;
        private ClientMessageModel currentClientMessageModel;
        private ObservableCollection<AccountEntity> accountList = new ObservableCollection<AccountEntity>();
        private AccountEntity selectedAccount;

        public MenuWindowViewModel()
        {
        }

        public ClientMessageModel CurrentClientMessageModel
        {
            get => currentClientMessageModel;
            set
            {
                if (Equals(currentClientMessageModel, value))
                    return;
                if (currentClientMessageModel != null)
                {
                    CurrentClientMessageModel.AccountClientManager.OnEntityCollectionChanged -= CurrentClientMessageModel_CurrentAccountCollectionUpdated;
                    CurrentClientMessageModel.LobbyClientManager.OnEntityCollectionChanged -= CurrentClientMessageModel_CurrentLobbyCollectionUpdated;

                    CurrentClientMessageModel.GameClientManager.OnGameCreated -= GameClientManager_OnGameCreated;
                }
                currentClientMessageModel = value;
                if (value != null)
                {
                    CurrentClientMessageModel.AccountClientManager.OnEntityCollectionChanged += CurrentClientMessageModel_CurrentAccountCollectionUpdated;
                    CurrentClientMessageModel.LobbyClientManager.OnEntityCollectionChanged += CurrentClientMessageModel_CurrentLobbyCollectionUpdated;
                    LobbyList = new ObservableCollection<LobbyEntity>(CurrentClientMessageModel.LobbyClientManager.InternalCollection);
                    AccountList = new ObservableCollection<AccountEntity>(CurrentClientMessageModel.AccountClientManager.InternalCollection);

                    CurrentClientMessageModel.GameClientManager.OnGameCreated += GameClientManager_OnGameCreated;
                }
            }
        }

        private void GameClientManager_OnGameCreated(object sender, CreateGameUpdate e)
        {
            //ToDo implement new window
            Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                GameWindowView newGameInstance = new GameWindowView(e.GameInstance.Id, CurrentClientMessageModel.GameClientManager);
                newGameInstance.Show();
            }));
        }

        private void CurrentClientMessageModel_CurrentLobbyCollectionUpdated(object sender, System.Collections.Generic.HashSet<LobbyEntity> e)
        {
            LobbyList = new ObservableCollection<LobbyEntity>(e);
        }

        private void CurrentClientMessageModel_CurrentAccountCollectionUpdated(object sender, System.Collections.Generic.HashSet<AccountEntity> e)
        {
            AccountList = new ObservableCollection<AccountEntity>(e);
        }

        public AccountEntity CurrentAccount { get; set; }

        public void SetConfiguration()
        {
            //throw new NotImplementedException("ToDo Implement configuration");
            MessageBox.Show(
                LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Not implemented in this version"),
                LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "This developer is working on other features right now."),
                MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        public void CreateLobby()
        {
            //saved entity will be rewritten, so it should be always different entity then the one which is selected.
            LobbyEntity savedEntity = new LobbyEntity("", 2);

            EntityEditorDialogView dialog = new EntityEditorDialogView(new LobbyConfigurationDialogView(savedEntity));
            dialog.ShowDialog();

            if (dialog.Result == EntityEditResult.SaveChanges)
                CurrentClientMessageModel.LobbyClientManager.SendRequestToPrimaryServer(savedEntity, CreateHandle, Request.Create);
        }

        public void JoinLobby()
        {
            CurrentClientMessageModel.LobbyClientManager.SendRequestToPrimaryServer(SelectedLobby, JoinHandle, Request.Join);
        }

        public void LeaveLobby()
        {
            CurrentClientMessageModel.LobbyClientManager.SendRequestToPrimaryServer(SelectedLobby, LeaveHandle, Request.Leave);
        }

        public void DeleteLobby()
        {
            CurrentClientMessageModel.LobbyClientManager.SendRequestToPrimaryServer(SelectedLobby, DeleteHandle, Request.Delete);
        }

        public void StartGame()
        {
            if (SelectedLobby.State == LobbyState.Waiting)
            {
                CurrentClientMessageModel.GameClientManager.SendCreateGameRequest(SelectedLobby, StartGameHandle);
            }
            else
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Game in progress"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "In this version it's not possible to connect during game in progress or start another game."),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void StartGameHandle(GameResponseMessage responseMessage)
        {
            var parsed = responseMessage as CreateGameResponse;
            if (parsed != null && parsed.WasCreated)
            {
                //this should be empty as we are answeoring only update event
                //CurrentClientMessageModel.GameClientManager.JoinGameResponse(parsed.GameInstance, StartGameHandle, GameRequest.JoinGame);
            }
            else
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Game in progress at selected lobby"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "In this version it's not possible to connect during game in progress or start another game."),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void LeaveHandle(ResponseMessage leaveResponseMessage)
        {
            var message = leaveResponseMessage as EntitySimpleResponseMessage<LobbyEntity>;
            if (message == null || message.Response == Response.Denied)
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Lobby was not left!"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "You can't leave this lobby when you are not in!"),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void DeleteHandle(ResponseMessage deleteResponseMessage)
        {
            var message = deleteResponseMessage as EntitySimpleResponseMessage<LobbyEntity>;
            if (message == null || message.Response == Response.Denied)
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Lobby was not deleted!"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "You can't delete this lobby yet!"),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void CreateHandle(ResponseMessage lobbyCreateResponse)
        {
            var message = lobbyCreateResponse as EntitySimpleResponseMessage<LobbyEntity>;
            if (message == null || message.Response == Response.Denied)
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Did not created Lobby!"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "You are not chosen to carry board!"),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                CurrentClientMessageModel.LobbyClientManager.SendRequestToPrimaryServer(message.Instance, JoinHandle, Request.Join);
            }
        }

        private void JoinHandle(ResponseMessage lobbyJoinResponse)
        {
            var message = lobbyJoinResponse as EntitySimpleResponseMessage<LobbyEntity>;
            if (message == null || message.Response == Response.Denied)
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Ha ha ha you failed to get in mon!"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "You failed to get on board!"),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                MessageBox.Show(
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "Joined Lobby!"),
                    LanguageHelper.TranslateContextual(nameof(MenuWindowViewModel), "You are chosen to carry board!"),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }


        public ObservableCollection<LobbyEntity> LobbyList
        {
            get => lobbyList;
            set
            {
                if (Equals(value, lobbyList)) return;
                lobbyList = value;
                OnPropertyChanged();
            }
        }

        public LobbyEntity SelectedLobby
        {
            get => selectedLobby;
            set
            {
                if (Equals(value, selectedLobby)) return;
                selectedLobby = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedLobby));
            }
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

        public bool HasSelectedLobby
        {
            get { return SelectedLobby != null; }
        }
    }
}