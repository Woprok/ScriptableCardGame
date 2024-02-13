using System;
using System.Linq;
using Shared.Networking.Common.Enums;
using Shared.Networking.Common.Exceptions;
using Shared.Networking.Common.Interfaces;
using Shared.Networking.Common.Managers;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Entities;
using Shared.Networking.Protocol.Enums;
using Shared.Networking.Protocol.Models;

namespace Shared.Networking.Protocol.Managers
{
    public sealed class LobbyServerManager : ServerEntityManager<LobbyEntity>
    {
        public LobbyServerManager(IDatabaseModel database, Action<CoreMessage> globalSendFunction) : base(database, globalSendFunction) { }

        private void EntityUpdated(LobbyEntity entity)
        {
            GlobalSendFunction.Invoke(GetCollectionMessage());
        }

        public void OnClientLost(AccountEntity account)
        {
            bool wasMember = false;
            lock (CollectionSynchronizationLock)
            {
                foreach (LobbyEntity lobby in InternalCollection.Where(lobby => lobby.CurrentPlayers.Contains(account)))
                {
                    lobby.CurrentPlayers.Remove(account);
                    wasMember = true;
                }
            }
            if (wasMember)
                EntityUpdated(null);
        }

        public bool TryLobbyStateChange(LobbyEntity lobby, bool toWaiting)
        {
            LobbyEntity foundLobby;
            lock (CollectionSynchronizationLock)
            {
                //ToDo verify that lobby settings are same
                foundLobby = InternalCollection.FirstOrDefault(item => item.Id == lobby.Id); 
                if (foundLobby != null)
                {
                    if (foundLobby.State == LobbyState.InProgress && toWaiting)
                    {
                        foundLobby.State = LobbyState.Waiting;
                    }
                    else if (foundLobby.State == LobbyState.Waiting && !toWaiting)
                    {
                        foundLobby.State = LobbyState.InProgress;
                    }
                    else
                    {
                        return false;
                    }
                    lobby.CurrentPlayers = foundLobby.CurrentPlayers; //provide server collection
                    EntityUpdated(null);
                    return true;
                }
            }
            return false;
        }

        public override void ParseRequestMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityRequestMessage requestMessage)
        {
            Response response = Response.Denied;
            var parsedMessage = requestMessage as EntitySimpleRequestMessage<LobbyEntity>;
            EntitySimpleResponseMessage<LobbyEntity> responseMessage;
            LobbyEntity lobbyEntity;

            switch (requestMessage.Request)
            {
                case Request.Create:
                    if (parsedMessage == null)
                        break;

                    lobbyEntity = Database.ValidateLobby(parsedMessage.Instance);

                    if (lobbyEntity != null)
                    {
                        response = Response.Accepted;
                    }

                    responseMessage = new EntitySimpleResponseMessage<LobbyEntity>(requestMessage) { Response = response, Instance = lobbyEntity };
                    exchangerModel.Send(responseMessage);

                    if (response == Response.Accepted)
                        EntityJoined(lobbyEntity);
                    break;
                case Request.Join:
                    if (parsedMessage == null)
                        break;
                    
                    lobbyEntity = InternalCollection.FirstOrDefault(item => item.Id == parsedMessage.Instance.Id);

                    if (lobbyEntity != null && !lobbyEntity.CurrentPlayers.Contains(exchangerModel.ReqisteredAccount) && lobbyEntity.CurrentPlayers.Count < lobbyEntity.MaxPlayerCount)
                    {
                        lobbyEntity.CurrentPlayers.Add(exchangerModel.ReqisteredAccount);
                        response = Response.Accepted;
                    }

                    responseMessage = new EntitySimpleResponseMessage<LobbyEntity>(requestMessage) { Response = response, Instance = parsedMessage.Instance };
                    exchangerModel.Send(responseMessage);

                    if (response == Response.Accepted)
                        EntityUpdated(responseMessage.Instance);
                    break;
                case Request.Delete:
                    if (parsedMessage == null)
                        break;

                    lobbyEntity = InternalCollection.FirstOrDefault(item => item.Id == parsedMessage.Instance.Id);

                    if (lobbyEntity != null && lobbyEntity.CurrentPlayers.Count == 0)
                    {
                        response = Response.Accepted;
                    }

                    responseMessage = new EntitySimpleResponseMessage<LobbyEntity>(requestMessage) { Response = response, Instance = parsedMessage.Instance };
                    exchangerModel.Send(responseMessage);

                    if (response == Response.Accepted)
                        EntityLeft(responseMessage.Instance);
                    break;
                case Request.Leave:
                    if (parsedMessage == null)
                        break;

                    lobbyEntity = InternalCollection.FirstOrDefault(item => item.Id == parsedMessage.Instance.Id);

                    if (lobbyEntity != null && lobbyEntity.CurrentPlayers.Contains(exchangerModel.ReqisteredAccount))
                    {
                        lobbyEntity.CurrentPlayers.Remove(exchangerModel.ReqisteredAccount);
                        response = Response.Accepted;
                    }

                    responseMessage = new EntitySimpleResponseMessage<LobbyEntity>(requestMessage) { Response = response, Instance = parsedMessage.Instance };
                    exchangerModel.Send(responseMessage);

                    if (response == Response.Accepted)
                        EntityUpdated(responseMessage.Instance);
                    break;
                case Request.List:
                    var message = GetCollectionMessage(requestMessage);
                    GlobalSendFunction.Invoke(message);
                    break;
                default:
                    throw new UnknownEnumValueException(nameof(requestMessage.Request));
            }
        }


        public override void ParseResponseMessage(ISendReceiveModel<CoreMessage> exchangerModel, EntityResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }
    }
}