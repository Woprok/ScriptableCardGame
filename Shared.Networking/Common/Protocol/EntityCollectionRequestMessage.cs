using System;
using System.Collections.Generic;
using Shared.Networking.Common.Enums;

namespace Shared.Networking.Common.Protocol
{
    [Serializable]
    public sealed class EntityCollectionRequestMessage<T> : EntityRequestMessage
    {
        public EntityCollectionRequestMessage(HashSet<T> instance, Request request)
        {
            Instance = instance;
            Request = request;
        }

        public HashSet<T> Instance { get; set; }
    }
}