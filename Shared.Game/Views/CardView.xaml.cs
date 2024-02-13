using System.Windows.Controls;
using Shared.Common.Languages;
using Shared.Game.Entities.Cards.Interfaces;

namespace Shared.Game.Views
{
    /// <summary>
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView : UserControl, ICardView, ILanguage
    {
        public ICardViewModel ContextModel { get; }

        public CardView(ICard card)
        {
            InitializeComponent();
            SetTranslations();

            ContextModel = DataContext as ICardViewModel;
            ContextModel.SetCard(card);
        }

        public void SetTranslations()
        {

        }
    }
}