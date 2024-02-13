using System.Windows.Controls;

namespace Shared.Common.Interfaces
{
    public interface IEntityEditorDialog : IEntityEditDialogActions
    {
        void SaveChanges();
        void DiscardChanges();
        ContentControl DisplayedModel { get; set; }
        bool CanSave { get; set; }
        void SetCanSave(object sender, bool newCanSaveState);
    }
}