using System;
using System.Collections.Generic;
using Shared.Networking.Common.Entities;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Protocol;

namespace Shared.Networking.Common.Managers
{
    /// <summary>
    /// This is pathetic try to make single manager to reduce amount of code (unsuccessfull) 
    /// ToDo make this stupid methods work better for detecting collection or something
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericEntityMessageManager<T> where T : UniqueEntity
    {
        protected readonly object CallbackSynchronizationLock = new object();
        protected readonly object CollectionSynchronizationLock = new object();
        public HashSet<T> InternalCollection { get; set; } = new HashSet<T>();
        /// <summary>
        /// This could potentially be Delegate for more generic approach
        /// </summary>
        protected Dictionary<Guid, Action<ResponseMessage>> CallbackDictionary { get; private set; } = new Dictionary<Guid, Action<ResponseMessage>>();

        protected void AddCallback(CallbackMessage message, Action<ResponseMessage> responseAction)
        {
            if (responseAction == null)
                return;

            message.CallbackGuid = Guid.NewGuid();

            lock (CallbackSynchronizationLock)
            {
                CallbackDictionary.Add(message.CallbackGuid.Value, responseAction);
            }
        }

        protected void InvokeCallback(ResponseMessage responseMessage)
        {
            if (responseMessage.CallbackGuid == null)
                return;

            Action<ResponseMessage> action = null;

            lock (CallbackSynchronizationLock)
            {
                CallbackDictionary.TryGetValue(responseMessage.CallbackGuid.Value, out action);
            }

            if (action == null)
                return;

            if (responseMessage.LastCallbackInvocation)
            {
                lock (CallbackSynchronizationLock)
                {
                    CallbackDictionary.Remove(responseMessage.CallbackGuid.Value);
                }
            }

            action.Invoke(responseMessage);
        }

        public RequestMessage CreateRequestMessage(Request request)
        {
            EntitySimpleRequestMessage<T> message = new EntitySimpleRequestMessage<T>(request);
            return message;
        }

        public RequestMessage CreateRequestMessage(T entity, Action<ResponseMessage> actionDelegate, Request request)
        {
            EntitySimpleRequestMessage<T> message = new EntitySimpleRequestMessage<T>(entity, request);
            AddCallback(message, actionDelegate);
            return message;
        }

        public RequestMessage CreateRequestMessage(HashSet<T> entity, Action<ResponseMessage> actionDelegate, Request request)
        {
            EntityCollectionRequestMessage<T> message = new EntityCollectionRequestMessage<T>(entity, request);
            AddCallback(message, actionDelegate);
            return message;
        }

        public abstract void ParseRequestMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityRequestMessage requestMessage); 
        public abstract void ParseResponseMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityResponseMessage responseMessage);
    }
}