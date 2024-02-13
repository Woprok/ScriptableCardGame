using System;
using System.Collections.ObjectModel;
using Shared.Common.Interfaces;
using Shared.Common.Languages;
using Shared.Common.Models;
using Shared.Networking.Protocol.Entities;

namespace Client.Launcher.ViewModels
{
    public sealed class LobbyConfigurationDialogViewModel : ViewModelBase, IEditableDialog<LobbyEntity>
    {
        private LobbyEntity editedLobbyEntity;
        private string lobbyNameValue;
        private int lobbyCurrentPlayerValue;
        private int lobbyMaxPlayerValue;
        private ObservableCollection<AccountEntity> accountList;
        private AccountEntity selectedAccount;

        public event EventHandler<bool> OnPropertyErrorChanged;

        public LobbyConfigurationDialogViewModel()
        {
        }
        
        ~LobbyConfigurationDialogViewModel()
        {
        }

        public void SetEditedEntity(LobbyEntity lobbyEntity)
        {
            editedLobbyEntity = lobbyEntity;
            LobbyNameValue = lobbyEntity.Name;
            LobbyCurrentPlayerValue = lobbyEntity.CurrentPlayers.Count;
            LobbyMaxPlayerValue = lobbyEntity.MaxPlayerCount;
            AccountList = new ObservableCollection<AccountEntity>(lobbyEntity.CurrentPlayers);
        }


        public override string this[string columnName]
        {
            get
            {
                if (columnName == nameof(LobbyNameValue))
                {
                    if (string.IsNullOrEmpty(LobbyNameValue))
                    {
                        OnPropertyErrorChanged?.Invoke(this, false);
                        return LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogViewModel), "Name can't be empty!");
                    }

                }

                if (columnName == nameof(LobbyMaxPlayerValue))
                {
                    if (LobbyMaxPlayerValue < 2 || LobbyMaxPlayerValue < LobbyCurrentPlayerValue)
                    {
                        OnPropertyErrorChanged?.Invoke(this, false);
                        return LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogViewModel), "Max player can't be lower then 2 or current player count!");
                    }

                }
                OnPropertyErrorChanged?.Invoke(this, true);
                UpdateEditedEntity();
                return base[columnName];
            }
        }

        private void UpdateEditedEntity()
        {
            if (LobbyNameValue != null)
                editedLobbyEntity.Name = LobbyNameValue;
            if (LobbyMaxPlayerValue >=2)
                editedLobbyEntity.MaxPlayerCount = LobbyMaxPlayerValue;
        }

        public string LobbyNameValue
        {
            get => lobbyNameValue;
            set
            {
                if (Equals(value, lobbyNameValue)) return;
                lobbyNameValue = value;
                OnPropertyChanged();
            }
        }

        public int LobbyCurrentPlayerValue
        {
            get => lobbyCurrentPlayerValue;
            set
            {
                if (Equals(value, lobbyCurrentPlayerValue)) return;
                lobbyCurrentPlayerValue = value;
                OnPropertyChanged();
            }
        }

        public int LobbyMaxPlayerValue
        {
            get => lobbyMaxPlayerValue;
            set
            {
                if (Equals(value, lobbyMaxPlayerValue)) return;
                lobbyMaxPlayerValue = value;
                OnPropertyChanged();
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
    }
}