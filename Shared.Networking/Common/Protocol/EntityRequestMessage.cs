using System;
using Shared.Networking.Common.Enums;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public class EntityRequestMessage : RequestMessage
    {
        public Request Request { get; set; }
    }
}