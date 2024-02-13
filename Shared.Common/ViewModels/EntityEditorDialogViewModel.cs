using System.Windows.Controls;
using Shared.Common.Enums;
using Shared.Common.Interfaces;
using Shared.Common.Models;

namespace Shared.Common.ViewModels
{
    public class EntityEditorDialogViewModel : EntityEditorDialogViewModelBase, IEntityEditorDialog
    {
        private bool canSave;
        private ContentControl displayedModel;

        public void SaveChanges() => InvokeCloseDialog(EntityEditResult.SaveChanges);
        public void DiscardChanges() => InvokeCloseDialog(EntityEditResult.DiscardChanges);

        public ContentControl DisplayedModel
        {
            get => displayedModel;
            set
            {
                if (Equals(value, displayedModel)) return;
                displayedModel = value;
                OnPropertyChanged();
            }
        }

        public bool CanSave
        {
            get => canSave;
            set
            {
                if (Equals(value, canSave)) return;
                canSave = value;
                OnPropertyChanged();
            }
        }

        public void SetCanSave(object sender, bool newCanSaveState)
        {
            CanSave = newCanSaveState;
        }
    }
}