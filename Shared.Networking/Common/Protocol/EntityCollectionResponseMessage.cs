using System;
using System.Collections.Generic;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public sealed class EntityCollectionResponseMessage<T> : EntityResponseMessage
    {
        public EntityCollectionResponseMessage(EntityRequestMessage request) : base(request) { }

        public HashSet<T> Instance { get; set; }
    }
}