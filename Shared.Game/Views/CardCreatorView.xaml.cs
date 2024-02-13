using System.Windows;
using System.Windows.Controls;
using Shared.Common.Languages;
using Shared.Game.Entities;
using Shared.Game.ViewModels;

namespace Shared.Game.Views
{
    /// <summary>
    /// Interaction logic for CardCreatorView.xaml
    /// </summary>
    public partial class CardCreatorView : UserControl, ILanguage, ICardCreatorView
    {
        public ICardCreator ContextModel { get; }

        public CardCreatorView(Player player)
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = DataContext as ICardCreator;
            ContextModel.ThisPlayer = player;
        }

        public void SetTranslations()
        {
            NewCard.Content = LanguageHelper.TranslateContextual(nameof(CardCreatorView), "New");
            TestCard.Content = LanguageHelper.TranslateContextual(nameof(CardCreatorView), "Test");
            ResetCard.Content = LanguageHelper.TranslateContextual(nameof(CardCreatorView), "Reset");
        }

        private void InvokeNewCard(object sender, RoutedEventArgs e) => ContextModel.NewCard();
        private void InvokeTestCard(object sender, RoutedEventArgs e) => ContextModel.TestCard();
        private void InvokeResetCard(object sender, RoutedEventArgs e) => ContextModel.ResetCard();
    }
}