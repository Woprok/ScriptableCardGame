using System.Windows.Controls;
using Shared.Common.Languages;
using Shared.Game.Interfaces;
using Shared.Game.ViewModels;

namespace Shared.Game.Views
{
    /// <summary>
    /// Interaction logic for GameBoardView.xaml
    /// </summary>
    public partial class GameBoardView : UserControl, IGameboardView, ILanguage
    {
        public IGameBoardViewModel ContextModel { get; }
        public GameBoardView()
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = DataContext as IGameBoardViewModel;
        }

        public void SetTranslations()
        {

        }
    }

    public interface IGameboardView
    {
        IGameBoardViewModel ContextModel { get; }
    }
}