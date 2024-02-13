using System.Windows;
using System.Windows.Threading;
using Client.Launcher.Interfaces;
using Shared.Common.Languages;

namespace Client.Launcher.Views
{
    /// <summary>
    /// Interaction logic for LoginWindowView.xaml
    /// </summary>
    public partial class LoginWindowView : Window, ILanguage
    {
        public ILoginWindow ContextModel;

        public LoginWindowView()
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = (ILoginWindow)DataContext;
            ContextModel.CloseWindow += HandleCloseWindow;
        }

        private void HandleCloseWindow() => Dispatcher.Invoke(Close, DispatcherPriority.Normal);

        private void InvokeTryLogin(object sender, RoutedEventArgs e) => ContextModel.TryLogin();

        private void InvokeConfirmServer(object sender, RoutedEventArgs e) => ContextModel.ConfirmServer();

        public void SetTranslations()
        {
            Title = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Project Woprok");
            IpAddressCaption.Text = LanguageHelper.TranslateContextual(nameof(LoginWindowView),"IpAdress");
            PortCaption.Text = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Port");
            ConfirmServer.Content = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Confirm Server");
            NameCaption.Text = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Account");
            PasswordCaption.Text = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Password");
            TryLogin.Content = LanguageHelper.TranslateContextual(nameof(LoginWindowView), "Login");
        }
    }
}