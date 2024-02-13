using System.Windows.Controls;
using Shared.Common.Interfaces;
using Shared.Common.Languages;
using Shared.Networking.Protocol.Entities;

namespace Client.Launcher.Views
{
    /// <summary>
    /// Interaction logic for LobbyConfigurationDialogView.xaml
    /// </summary>
    public partial class LobbyConfigurationDialogView : UserControl, ILanguage
    {
        public IEditableDialog<LobbyEntity> ContextModel;

        public LobbyConfigurationDialogView(LobbyEntity entity)
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = (IEditableDialog<LobbyEntity>)DataContext;
            ContextModel.SetEditedEntity(entity);
        }
        
        public void SetTranslations()
        {
            LobbyNameCaption.Text = LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogView), "Lobby name:");
            LobbyCurrentPlayerCaption.Text = LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogView), "Current Player count");
            LobbyMaxPlayerCaption.Text = LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogView), "Max player count");
            AccountNameCaption.Header = LanguageHelper.TranslateContextual(nameof(LobbyConfigurationDialogView), "Player");
        }
    }
}