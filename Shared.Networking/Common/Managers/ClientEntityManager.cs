using System;
using System.Collections.Generic;
using Shared.Networking.Common.Entities;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Protocol;

namespace Shared.Networking.Common.Managers
{
    public abstract class ClientEntityManager<T> : GenericEntityMessageManager<T> where T : UniqueEntity
    {
        public EventHandler<HashSet<T>> OnEntityCollectionChanged;

        protected ClientEntityManager(Action<CoreMessage> serverSendFunction)
        {
            ServerSendFunction = serverSendFunction;
        }

        protected void UpdateBuddies(HashSet<T> collection)
        {
            InternalCollection = collection;
            OnEntityCollectionChanged?.Invoke(this, InternalCollection);
        }

        protected Action<CoreMessage> ServerSendFunction { get; }

        public void SendRequestToPrimaryServer(Request request)
        {
            ServerSendFunction?.Invoke(CreateRequestMessage(request));
        }

        public void SendRequestToPrimaryServer(T entity, Action<ResponseMessage> actionDelegate, Request request)
        {
            ServerSendFunction?.Invoke(CreateRequestMessage(entity, actionDelegate, request));
        }

        public void SendRequestToPrimaryServer(HashSet<T> entity, Action<ResponseMessage> actionDelegate, Request request)
        {
            ServerSendFunction?.Invoke(CreateRequestMessage(entity, actionDelegate, request));
        }
    }
}