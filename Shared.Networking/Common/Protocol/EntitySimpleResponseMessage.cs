using System;

namespace Shared.Networking.Common.Protocol
{
    /// <summary>
    /// Generic answer for EntitySimpleRequestMessage.
    /// </summary>
    [Serializable]
    public sealed class EntitySimpleResponseMessage<T> : EntityResponseMessage
    {
        public EntitySimpleResponseMessage(EntityRequestMessage simpleRequest) : base(simpleRequest) { }

        public T Instance { get; set; }
    }
}