using System.Windows;
using System.Windows.Threading;
using Client.Launcher.Interfaces;
using Shared.Common.Languages;

namespace Client.Launcher.Views
{
    /// <summary>
    /// Interaction logic for MenuWindowView.xaml
    /// </summary>
    public partial class MenuWindowView : Window, ILanguage
    {
        public IMenuWindow ContextModel;

        public MenuWindowView()
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = (IMenuWindow)DataContext;
            ContextModel.CloseWindow += HandleCloseWindow;
        }

        private void HandleCloseWindow() => Dispatcher.Invoke(Close, DispatcherPriority.Normal);

        private void InvokeSetConfiguration(object sender, RoutedEventArgs e) => ContextModel.SetConfiguration();

        private void InvokeCreateLobby(object sender, RoutedEventArgs e) => ContextModel.CreateLobby();
        private void InvokeJoinLobby(object sender, RoutedEventArgs e) => ContextModel.JoinLobby();
        private void InvokeLeaveLobby(object sender, RoutedEventArgs e) => ContextModel.LeaveLobby();
        private void InvokeDeleteLobby(object sender, RoutedEventArgs e) => ContextModel.DeleteLobby();
        private void InvokeStartGame(object sender, RoutedEventArgs e) => ContextModel.StartGame();

        public void SetTranslations()
        {
            Title = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Project Woprok");
            LargeTitle.Text = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Project Woprok");
            SetConfiguration.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Configure");
            CreateLobby.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "New lobby");
            JoinLobby.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Join lobby");
            LeaveLobby.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Leave lobby");
            DeleteLobby.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Delete lobby");
            StartGame.Content = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Start game");
            LobbyNameCaption.Header = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Lobby Name");
            LobbyCurrentPlayerCountCaption.Header = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Min");
            LobbyMaxPlayerCountCaption.Header = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Max");
            LobbyStateCaption.Header = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "State");
            AccountNameCaption.Header = LanguageHelper.TranslateContextual(nameof(MenuWindowView), "Player");
        }
    }
}