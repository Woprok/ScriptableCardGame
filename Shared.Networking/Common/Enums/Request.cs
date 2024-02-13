using System;

namespace Shared.Networking.Common.Enums
{
    [Serializable]
    public enum Request
    {
        Create,
        Delete,
        Join,
        Leave,
        List,
    }
}