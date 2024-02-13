using System;
using System.Net;
using System.Windows;
using Client.Launcher.Interfaces;
using Client.Launcher.Models;
using Shared.Common.Languages;
using Shared.Common.Models;
using Shared.Common.StateMachines;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Models;

namespace Client.Launcher.ViewModels
{
    public sealed class LoginWindowViewModel : WindowViewModelBase, ILoginWindow
    {
        private IPAddress ipAddress = IPAddress.Loopback;
        private string ipAddressValue = IPAddress.Loopback.ToString();
        private int port = 1996;
        private string portValue = 1996.ToString();
        private string nameValue;
        private string passwordValue;
        private bool canConfirmServer = false;
        private bool serverNotSelected = true;
        private bool loginAvailable = false;
        private bool canTryLogin = false;

        public LoginWindowViewModel()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2) nameValue = args[1];
            if (args.Length >= 3) passwordValue = args[2];

            StateMachine = new LoginStateMachineModel(LoginState.Initialized)
            {
                OnServerConfirmed = OnServerConfirmed,
                OnLoginFailure = OnLoginFailure,
                OnLoginSuccess = OnLoginSuccess,
            };
        }

        public void ConfirmServer()
        {
            StateMachine.ProcessTrigger(LoginTrigger.ServerSelected);
            CanConfirmServer = false;
            ServerNotSelected = false;
            LoginAvailable = true;
            if (!string.IsNullOrEmpty(NameValue))
                CanTryLogin = true;
        }

        private void OnServerConfirmed()
        {
            ClientMessageModel = new ClientMessageModel(new IPEndPoint(ipAddress, port));
        }

        public void TryLogin()
        {
            CanTryLogin = false;
            AccountEntity = new AccountEntity(NameValue, PasswordValue);
            ClientMessageModel.AccountClientManager.SendRequestToPrimaryServer(AccountEntity, LoginHandle, Request.Join);
        }

        private void OnLoginFailure()
        {
            CanTryLogin = true;
            MessageBox.Show(
                LanguageHelper.TranslateContextual(nameof(LoginWindowViewModel), "Ha ha ha you failed to get in mon!"),
                LanguageHelper.TranslateContextual(nameof(LoginWindowViewModel), "You failed to get on board!"),
                MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void OnLoginSuccess()
        {
            InvokeCloseWindow();
        }

        private void LoginHandle(ResponseMessage loginResponse)
        {
            EntityResponseMessage loginResponseMessage = (EntityResponseMessage)loginResponse;
            switch (loginResponseMessage.Response)
            {
                case Response.Accepted:
                    StateMachine.ProcessTrigger(LoginTrigger.LoginSuccess);
                    break;
                case Response.Denied:
                    StateMachine.ProcessTrigger(LoginTrigger.LoginFailure);
                    break;
                default:
                    throw new UnknownEnumValueException(nameof(loginResponseMessage.Response));
            }
        }

        public StateMachine<LoginState, LoginTrigger> StateMachine { get; private set; }
        public AccountEntity AccountEntity { get; private set; }
        public ClientMessageModel ClientMessageModel { get; private set; }

        public override string this[string columnName]
        {
            get
            {
                CanConfirmServer = false;

                if (columnName == nameof(IpAddressValue))
                {
                    if (!IPAddress.TryParse(IpAddressValue, out ipAddress))
                        return LanguageHelper.TranslateContextual(nameof(LoginWindowViewModel), "Incorrect address!");
                }
                if (columnName == nameof(PortValue))
                {
                    if (!int.TryParse(PortValue, out port) || port < 0 || port > 65535)
                        return LanguageHelper.TranslateContextual(nameof(LoginWindowViewModel), "Incorrect port value!");
                }
                CanConfirmServer = true;

                CanTryLogin = false;
                if (columnName == nameof(NameValue))
                {
                    if (string.IsNullOrEmpty(NameValue))
                        return LanguageHelper.TranslateContextual(nameof(LoginWindowViewModel), "Enter name!");
                }

                CanTryLogin = true;

                return base[columnName];
            }
        }

        public string IpAddressValue
        {
            get => ipAddressValue;
            set
            {
                if (value == ipAddressValue) return;
                ipAddressValue = value;
                OnPropertyChanged();
            }
        }

        public string PortValue
        {
            get => portValue;
            set
            {
                if (value == portValue) return;
                portValue = value;
                OnPropertyChanged();
            }
        }

        public string NameValue
        {
            get => nameValue;
            set
            {
                if (value == nameValue) return;
                nameValue = value;
                OnPropertyChanged();
            }
        }

        public string PasswordValue
        {
            get => passwordValue;
            set
            {
                if (value == passwordValue) return;
                passwordValue = value;
                OnPropertyChanged();
            }
        }

        public bool CanConfirmServer
        {
            get => canConfirmServer;
            set
            {
                if (value == canConfirmServer) return;
                canConfirmServer = value;
                OnPropertyChanged();
            }
        }

        public bool ServerNotSelected
        {
            get => serverNotSelected;
            set
            {
                if (value == serverNotSelected) return;
                serverNotSelected = value;
                OnPropertyChanged();
            }
        }

        public bool LoginAvailable
        {
            get => loginAvailable;
            set
            {
                if (value == loginAvailable) return;
                loginAvailable = value;
                OnPropertyChanged();
            }
        }

        public bool CanTryLogin
        {
            get => canTryLogin;
            set
            {
                if (value == canTryLogin) return;
                canTryLogin = value;
                OnPropertyChanged();
            }
        }
    }
}