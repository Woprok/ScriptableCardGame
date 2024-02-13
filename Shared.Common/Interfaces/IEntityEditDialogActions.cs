using System;
using Shared.Common.Enums;

namespace Shared.Common.Interfaces
{
    public interface IEntityEditDialogActions
    {
        event Action<EntityEditResult> CloseDialog;
    }
}