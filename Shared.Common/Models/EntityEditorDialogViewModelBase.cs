using System;
using Shared.Common.Enums;
using Shared.Common.Interfaces;

namespace Shared.Common.Models
{
    public abstract class EntityEditorDialogViewModelBase : ViewModelBase, IEntityEditDialogActions
    {
        public event Action<EntityEditResult> CloseDialog;
        protected virtual void InvokeCloseDialog(EntityEditResult result) => CloseDialog?.Invoke(result);
    }
}