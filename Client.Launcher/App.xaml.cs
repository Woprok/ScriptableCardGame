using System;
using System.Windows;
using Client.Launcher.Views;
using Shared.Common.Languages;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Models;

namespace Client.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BootStrapper BootStrapper;
        public static AccountEntity CurrentAccount;
        public static ClientMessageModel CurrentClientMessageModel;
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Exit += App_Exit;

            BootStrapper = new BootStrapper();
            BootStrapper.StartHandlingUnhandledException();

            BootStrapper.CreateWindows();

            BootStrapper.LoginWindow.Activate();
            BootStrapper.LoginWindow.ShowDialog();

            //ToDo better way of giving MenuWindow information obtained in LoginWindow
            CurrentClientMessageModel = BootStrapper.LoginWindow.ContextModel.ClientMessageModel;
            CurrentAccount = BootStrapper.LoginWindow.ContextModel.AccountEntity;

            if (CurrentAccount == null) //No account should be equal to exit
                return;

            BootStrapper.MenuWindow.ContextModel.CurrentClientMessageModel = CurrentClientMessageModel;
            BootStrapper.MenuWindow.ContextModel.CurrentAccount = CurrentAccount;

            BootStrapper.MenuWindow.Activate();
            BootStrapper.MenuWindow.ShowDialog();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            CurrentClientMessageModel?.Stop();
        }
    }

    public sealed class BootStrapper
    {
        public static LoginWindowView LoginWindow;
        public static MenuWindowView MenuWindow;

        public void CreateWindows()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            MenuWindow = new MenuWindowView();
            LoginWindow = new LoginWindowView();
        }

        public void StartHandlingUnhandledException()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //ToDo Logging
            MessageBox.Show(
                LanguageHelper.TranslateContextual(nameof(BootStrapper), "An unexpected error occured and has been logged. Application will be terminated."), 
                LanguageHelper.TranslateContextual(nameof(BootStrapper), "Emergency application termination"), 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}