using System;
using System.Windows;
using System.Windows.Threading;
using Client.Launcher.GameControls.Interfaces;
using Shared.Common.Languages;
using Shared.Networking.Game.Managers;

namespace Client.Launcher.GameControls.Views
{
    /// <summary>
    /// Interaction logic for GameWindowView.xaml
    /// </summary>
    public partial class GameWindowView : Window, ILanguage
    {
        public IGameWindow ContextModel;

        public GameWindowView(Guid gameId, GameClientManager gameManager)
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = (IGameWindow)DataContext;
            ContextModel.CloseWindow += HandleCloseWindow;
            Closing += (sender, e) => ContextModel.OnUserCloseWindow();
            ContextModel.SetGameManager(gameId, gameManager);
            Loaded += (sender, e) => ContextModel.OnActivate();
        }

        private void HandleCloseWindow() => Dispatcher.Invoke(Close, DispatcherPriority.Normal);

        public void SetTranslations()
        {
            Title = LanguageHelper.TranslateContextual(nameof(GameWindowView), "Project Woprok");
            AccountNameCaption.Header = LanguageHelper.TranslateContextual(nameof(GameWindowView), "Connected Players");
            NextTurn.Content = LanguageHelper.TranslateContextual(nameof(GameWindowView), "Next Turn");
            CurrentPlayerCaption.Text = LanguageHelper.TranslateContextual(nameof(GameWindowView), "Current Player Turn: ");
        }

        private void InvokeNextTurn(object sender, RoutedEventArgs e)
        {
            ContextModel.NextTurn();
        }
    }
}