using System;

namespace Shared.Common.Interfaces
{
    public interface IEditDialog
    {
        event EventHandler<bool> OnPropertyErrorChanged;
    }
    public interface IEditableDialog<T> : IEditDialog
    {
        void SetEditedEntity(T lobbyEntity);
    }
}