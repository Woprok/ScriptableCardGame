using System;
using Shared.Common.Interfaces;

namespace Shared.Common.Models
{
    public abstract class WindowViewModelBase : ViewModelBase, IWindowActions
    {
        public event Action CloseWindow;

        protected virtual void InvokeCloseWindow() => CloseWindow?.Invoke();
    }
}