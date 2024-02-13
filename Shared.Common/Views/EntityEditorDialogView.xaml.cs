using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Shared.Common.Enums;
using Shared.Common.Interfaces;
using Shared.Common.Languages;

namespace Shared.Common.Views
{
    /// <summary>
    /// Interaction logic for EntityEditorDialogView.xaml
    /// </summary>
    public partial class EntityEditorDialogView : Window, ILanguage
    {
        private IEntityEditorDialog ContextModel;

        public EntityEditorDialogView(ContentControl model)
        {
            InitializeComponent();
            SetTranslations();
            ContextModel = (IEntityEditorDialog) DataContext;
            ContextModel.CloseDialog += HandleCloseWindow;
            ContextModel.DisplayedModel = model;
            ((IEditDialog)ContextModel.DisplayedModel.DataContext).OnPropertyErrorChanged += ContextModel.SetCanSave;
        }

        private void HandleCloseWindow(EntityEditResult entityEditResult)
        {
            Result = entityEditResult;
            Dispatcher.Invoke(Close, DispatcherPriority.Normal);
        }

        public void SetTranslations()
        {
            SaveChanges.Content = LanguageHelper.TranslateContextual(nameof(EntityEditorDialogView), "Save");
            DiscardChanges.Content = LanguageHelper.TranslateContextual(nameof(EntityEditorDialogView), "Cancel");
        }

        private void InvokeSaveChanges(object sender, RoutedEventArgs e) => ContextModel.SaveChanges();
        private void InvokeCancelChanges(object sender, RoutedEventArgs e) => ContextModel.DiscardChanges();

        public EntityEditResult Result { get; private set; } = EntityEditResult.DiscardChanges;
    }
}