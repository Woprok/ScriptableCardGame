using System;
using Shared.Networking.Common.Entities;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Models;

namespace Shared.Networking.Common.Managers
{
    public abstract class ServerEntityManager<T> : GenericEntityMessageManager<T> where T : UniqueEntity
    {
        protected ServerEntityManager(IDatabaseModel database, Action<CoreMessage> globalSendFunction)
        {
            Database = database;
            GlobalSendFunction = globalSendFunction;
        }

        protected IDatabaseModel Database { get; }
        protected Action<CoreMessage> GlobalSendFunction { get; }

        public void EntityJoined(T entity)
        {
            bool wasAdded = false;
            lock (CollectionSynchronizationLock)
            {
                wasAdded = InternalCollection.Add(entity);
            }
            if (wasAdded)
                GlobalSendFunction.Invoke(GetCollectionMessage());
        }

        public void EntityLeft(T entity)
        {
            bool wasRemoved = false;
            lock (CollectionSynchronizationLock)
            {
                wasRemoved = InternalCollection.Remove(entity);
            }
            if (wasRemoved)
                GlobalSendFunction.Invoke(GetCollectionMessage());
        }

        protected EntityCollectionResponseMessage<T> GetCollectionMessage(EntityRequestMessage requestMessage = null) =>
            new EntityCollectionResponseMessage<T>(requestMessage)
            {
                Instance = InternalCollection,
                Response = Response.Update
            };
    }
}