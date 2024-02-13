using System;
using Shared.Networking.Common.Enums;

namespace Shared.Networking.Common.Protocol
{
    /// <summary>
    /// Generic message for entity T, to simplify most common tasks, message is parsed based on typeof(T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class EntitySimpleRequestMessage<T> : EntityRequestMessage
    {
        public EntitySimpleRequestMessage(Request request)
        {
            Request = request;
        }

        public EntitySimpleRequestMessage(T instance, Request request) : this(request)
        {
            Instance = instance;
        }

        public T Instance { get; set; }
    }
}